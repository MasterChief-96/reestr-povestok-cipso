using Cipso.Registry.Api.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cipso.Registry.Api.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260928143000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "AuthorityOffices",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                Name = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                Region = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_AuthorityOffices", x => x.Id));

        migrationBuilder.CreateTable(
            name: "Citizens",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                RegistryNumber = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                MiddleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                Phone = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_Citizens", x => x.Id));

        migrationBuilder.CreateTable(
            name: "Employees",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                AuthorityOfficeId = table.Column<Guid>(type: "uuid", nullable: false),
                PersonnelNumber = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                FullName = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                Role = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Employees", x => x.Id);
                table.ForeignKey(
                    name: "FK_Employees_AuthorityOffices_AuthorityOfficeId",
                    column: x => x.AuthorityOfficeId,
                    principalTable: "AuthorityOffices",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Addresses",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CitizenId = table.Column<Guid>(type: "uuid", nullable: false),
                PostalCode = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                Region = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                City = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                Street = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                Building = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                Apartment = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Addresses", x => x.Id);
                table.ForeignKey(
                    name: "FK_Addresses_Citizens_CitizenId",
                    column: x => x.CitizenId,
                    principalTable: "Citizens",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Summonses",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                CitizenId = table.Column<Guid>(type: "uuid", nullable: false),
                AuthorityOfficeId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedByEmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                IssuedAt = table.Column<DateOnly>(type: "date", nullable: false),
                DueAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                Status = table.Column<string>(type: "text", nullable: false),
                Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Summonses", x => x.Id);
                table.ForeignKey(
                    name: "FK_Summonses_AuthorityOffices_AuthorityOfficeId",
                    column: x => x.AuthorityOfficeId,
                    principalTable: "AuthorityOffices",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Summonses_Citizens_CitizenId",
                    column: x => x.CitizenId,
                    principalTable: "Citizens",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Summonses_Employees_CreatedByEmployeeId",
                    column: x => x.CreatedByEmployeeId,
                    principalTable: "Employees",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Appeals",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                SummonsId = table.Column<Guid>(type: "uuid", nullable: false),
                Type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                Text = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                Status = table.Column<string>(type: "text", nullable: false),
                SubmittedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Appeals", x => x.Id);
                table.ForeignKey(
                    name: "FK_Appeals_Summonses_SummonsId",
                    column: x => x.SummonsId,
                    principalTable: "Summonses",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AuditEvents",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                SummonsId = table.Column<Guid>(type: "uuid", nullable: false),
                Action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Actor = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                OccurredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                Details = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AuditEvents", x => x.Id);
                table.ForeignKey(
                    name: "FK_AuditEvents_Summonses_SummonsId",
                    column: x => x.SummonsId,
                    principalTable: "Summonses",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Documents",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                SummonsId = table.Column<Guid>(type: "uuid", nullable: false),
                FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                MimeType = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                StorageUri = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Documents", x => x.Id);
                table.ForeignKey(
                    name: "FK_Documents_Summonses_SummonsId",
                    column: x => x.SummonsId,
                    principalTable: "Summonses",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Notifications",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                SummonsId = table.Column<Guid>(type: "uuid", nullable: false),
                Channel = table.Column<string>(type: "text", nullable: false),
                DestinationMasked = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                Status = table.Column<string>(type: "text", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Notifications", x => x.Id);
                table.ForeignKey(
                    name: "FK_Notifications_Summonses_SummonsId",
                    column: x => x.SummonsId,
                    principalTable: "Summonses",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "SummonsStatusHistory",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                SummonsId = table.Column<Guid>(type: "uuid", nullable: false),
                FromStatus = table.Column<string>(type: "text", nullable: true),
                ToStatus = table.Column<string>(type: "text", nullable: false),
                ChangedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                ChangedBy = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                Comment = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SummonsStatusHistory", x => x.Id);
                table.ForeignKey(
                    name: "FK_SummonsStatusHistory_Summonses_SummonsId",
                    column: x => x.SummonsId,
                    principalTable: "Summonses",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "DeliveryAttempts",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                NotificationId = table.Column<Guid>(type: "uuid", nullable: false),
                AttemptNumber = table.Column<int>(type: "integer", nullable: false),
                Result = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                AttemptedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                ProviderMessage = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DeliveryAttempts", x => x.Id);
                table.ForeignKey(
                    name: "FK_DeliveryAttempts_Notifications_NotificationId",
                    column: x => x.NotificationId,
                    principalTable: "Notifications",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex("IX_Addresses_CitizenId", "Addresses", "CitizenId", unique: true);
        migrationBuilder.CreateIndex("IX_Appeals_SummonsId", "Appeals", "SummonsId");
        migrationBuilder.CreateIndex("IX_AuditEvents_SummonsId", "AuditEvents", "SummonsId");
        migrationBuilder.CreateIndex("IX_AuthorityOffices_Code", "AuthorityOffices", "Code", unique: true);
        migrationBuilder.CreateIndex("IX_Citizens_RegistryNumber", "Citizens", "RegistryNumber", unique: true);
        migrationBuilder.CreateIndex("IX_DeliveryAttempts_NotificationId", "DeliveryAttempts", "NotificationId");
        migrationBuilder.CreateIndex("IX_Documents_SummonsId", "Documents", "SummonsId");
        migrationBuilder.CreateIndex("IX_Employees_AuthorityOfficeId", "Employees", "AuthorityOfficeId");
        migrationBuilder.CreateIndex("IX_Employees_PersonnelNumber", "Employees", "PersonnelNumber", unique: true);
        migrationBuilder.CreateIndex("IX_Notifications_SummonsId", "Notifications", "SummonsId");
        migrationBuilder.CreateIndex("IX_Summonses_AuthorityOfficeId", "Summonses", "AuthorityOfficeId");
        migrationBuilder.CreateIndex("IX_Summonses_CitizenId", "Summonses", "CitizenId");
        migrationBuilder.CreateIndex("IX_Summonses_CreatedByEmployeeId", "Summonses", "CreatedByEmployeeId");
        migrationBuilder.CreateIndex("IX_Summonses_Number", "Summonses", "Number", unique: true);
        migrationBuilder.CreateIndex("IX_SummonsStatusHistory_SummonsId", "SummonsStatusHistory", "SummonsId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("Addresses");
        migrationBuilder.DropTable("Appeals");
        migrationBuilder.DropTable("AuditEvents");
        migrationBuilder.DropTable("DeliveryAttempts");
        migrationBuilder.DropTable("Documents");
        migrationBuilder.DropTable("SummonsStatusHistory");
        migrationBuilder.DropTable("Notifications");
        migrationBuilder.DropTable("Summonses");
        migrationBuilder.DropTable("Citizens");
        migrationBuilder.DropTable("Employees");
        migrationBuilder.DropTable("AuthorityOffices");
    }
}
