using Cipso.Registry.Api.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cipso.Registry.Api.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260928203000_AddCitizenWriteOff")]
public partial class AddCitizenWriteOff : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsWrittenOff",
            table: "Citizens",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "WrittenOffAt",
            table: "Citizens",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "WrittenOffBy",
            table: "Citizens",
            type: "character varying(180)",
            maxLength: 180,
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "IsWrittenOff", table: "Citizens");
        migrationBuilder.DropColumn(name: "WrittenOffAt", table: "Citizens");
        migrationBuilder.DropColumn(name: "WrittenOffBy", table: "Citizens");
    }
}
