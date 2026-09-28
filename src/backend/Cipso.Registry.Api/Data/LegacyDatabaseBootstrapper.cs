using System.Data;
using Microsoft.EntityFrameworkCore;

namespace Cipso.Registry.Api.Data;

public static class LegacyDatabaseBootstrapper
{
    private const string InitialMigrationId = "20260928143000_InitialCreate";
    private const string ProductVersion = "8.0.4";

    private static readonly string[] RequiredLegacyTables =
    [
        "Addresses",
        "Appeals",
        "AuditEvents",
        "AuthorityOffices",
        "Citizens",
        "DeliveryAttempts",
        "Documents",
        "Employees",
        "Notifications",
        "Summonses",
        "SummonsStatusHistory"
    ];

    public static async Task PrepareAsync(
        AppDbContext db,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        var connection = db.Database.GetDbConnection();
        var shouldClose = connection.State != ConnectionState.Open;

        if (shouldClose)
            await connection.OpenAsync(cancellationToken);

        try
        {
            var existingTables = new HashSet<string>(StringComparer.Ordinal);

            await using (var command = connection.CreateCommand())
            {
                command.CommandText = """
                    SELECT tablename
                    FROM pg_catalog.pg_tables
                    WHERE schemaname = 'public';
                    """;

                await using var reader = await command.ExecuteReaderAsync(cancellationToken);
                while (await reader.ReadAsync(cancellationToken))
                    existingTables.Add(reader.GetString(0));
            }

            var historyTableExists = existingTables.Contains("__EFMigrationsHistory");

            if (historyTableExists)
            {
                await using var checkMigration = connection.CreateCommand();
                checkMigration.CommandText = """
                    SELECT EXISTS (
                        SELECT 1
                        FROM "__EFMigrationsHistory"
                        WHERE "MigrationId" = @migrationId
                    );
                    """;

                var migrationParameter = checkMigration.CreateParameter();
                migrationParameter.ParameterName = "migrationId";
                migrationParameter.Value = InitialMigrationId;
                checkMigration.Parameters.Add(migrationParameter);

                var migrationApplied = await checkMigration.ExecuteScalarAsync(cancellationToken);
                if (migrationApplied is true)
                    return;
            }

            var existingLegacyTables = RequiredLegacyTables
                .Where(existingTables.Contains)
                .ToArray();

            // Empty/new database: let EF Core create the history table and apply the migration normally.
            if (existingLegacyTables.Length == 0)
                return;

            if (existingLegacyTables.Length != RequiredLegacyTables.Length)
            {
                var missing = RequiredLegacyTables
                    .Except(existingLegacyTables, StringComparer.Ordinal);

                throw new InvalidOperationException(
                    "Detected a partial legacy database schema without the initial migration registered. " +
                    $"Missing tables: {string.Join(", ", missing)}. " +
                    "For the demo environment, recreate the local database with 'docker compose down -v' " +
                    "and then 'docker compose up --build -d'.");
            }

            logger.LogWarning(
                "Legacy EnsureCreated database detected. Registering initial EF migration {MigrationId} as applied.",
                InitialMigrationId);

            await using var bootstrap = connection.CreateCommand();
            bootstrap.CommandText = """
                CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
                    "MigrationId" character varying(150) NOT NULL,
                    "ProductVersion" character varying(32) NOT NULL,
                    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
                );

                INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
                VALUES (@migrationId, @productVersion)
                ON CONFLICT ("MigrationId") DO NOTHING;
                """;

            var migrationIdParameter = bootstrap.CreateParameter();
            migrationIdParameter.ParameterName = "migrationId";
            migrationIdParameter.Value = InitialMigrationId;
            bootstrap.Parameters.Add(migrationIdParameter);

            var productVersionParameter = bootstrap.CreateParameter();
            productVersionParameter.ParameterName = "productVersion";
            productVersionParameter.Value = ProductVersion;
            bootstrap.Parameters.Add(productVersionParameter);

            await bootstrap.ExecuteNonQueryAsync(cancellationToken);
        }
        finally
        {
            if (shouldClose)
                await connection.CloseAsync();
        }
    }
}
