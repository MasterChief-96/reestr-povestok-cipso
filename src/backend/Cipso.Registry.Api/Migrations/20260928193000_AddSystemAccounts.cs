using Cipso.Registry.Api.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cipso.Registry.Api.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260928193000_AddSystemAccounts")]
public partial class AddSystemAccounts : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "SystemAccounts",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                ExternalSubject = table.Column<string>(type: "character varying(96)", maxLength: 96, nullable: false),
                DisplayName = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                Role = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                CitizenRegistryNumber = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_SystemAccounts", x => x.Id));

        migrationBuilder.CreateIndex(
            name: "IX_SystemAccounts_ExternalSubject",
            table: "SystemAccounts",
            column: "ExternalSubject",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "SystemAccounts");
    }
}
