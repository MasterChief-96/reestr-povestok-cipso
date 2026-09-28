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

            if (existingTables.Contains("__EFMigrationsHistory"))
                return;

            var existingLegacyTables = RequiredLegacyTables
                .Where(existingTables.Contains)
                .ToArray();

            if (existingLegacyTables.Length == 0)
                return;

            if (existingLegacyTables.Length != RequiredLegacyTables.Length)
            {
                var missing = RequiredLegacyTables
                    .Except(existingLegacyTables, StringComparer.Ordinal);

                throw new InvalidOperationException(
                    "Detected a partial legacy database schema without EF migration history. " +
                    $"Missing tables: {string.Join(", ", missing)}. " +
                    "For the demo environment, recreate the local database with 'docker compose down -v' " +
                    "and then 'docker compose up --build -d'.");
            }

            logger.LogWarning(
                "Legacy EnsureCreated database detected. Registering initial EF migration {MigrationId} as applied.",
                InitialMigrationId);

            await using var bootstrap = connection.CreateCommand();
            bootstrap.CommandText = $"""
                CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
                    "MigrationId" character varying(150) NOT NULL,
                    "ProductVersion" character varying(32) NOT NULL,
                    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
                );

                INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
                VALUES ('{InitialMigrationId}', '{ProductVersion}')
                ON CONFLICT ("MigrationId") DO NOTHING;
                """;

            await bootstrap.ExecuteNonQueryAsync(cancellationToken);
        }
        finally
        {
            if (shouldClose)
                await connection.CloseAsync();
        }
    }
}
