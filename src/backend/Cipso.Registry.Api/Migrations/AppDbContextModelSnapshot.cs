using Cipso.Registry.Api.Data;
using Cipso.Registry.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cipso.Registry.Api.Migrations;

[DbContext(typeof(AppDbContext))]
public partial class AppDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "8.0.8");

        modelBuilder.Entity<Citizen>(b =>
        {
            b.Property<Guid>(x => x.Id).ValueGeneratedOnAdd();
            b.Property<string>(x => x.RegistryNumber).HasMaxLength(64).IsRequired();
            b.Property<string>(x => x.LastName).HasMaxLength(100).IsRequired();
            b.Property<string>(x => x.FirstName).HasMaxLength(100).IsRequired();
            b.Property<string?>(x => x.MiddleName).HasMaxLength(100);
            b.Property<DateOnly>(x => x.BirthDate);
            b.Property<string?>(x => x.Email).HasMaxLength(255);
            b.Property<string?>(x => x.Phone).HasMaxLength(32);
            b.Property<bool>(x => x.IsWrittenOff);
            b.Property<DateTimeOffset?>(x => x.WrittenOffAt);
            b.Property<string?>(x => x.WrittenOffBy).HasMaxLength(180);
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.RegistryNumber).IsUnique();
            b.ToTable("Citizens");
        });

        modelBuilder.Entity<Address>(b =>
        {
            b.Property<Guid>(x => x.Id).ValueGeneratedOnAdd();
            b.Property<Guid>(x => x.CitizenId);
            b.Property<string>(x => x.PostalCode).HasMaxLength(16).IsRequired();
            b.Property<string>(x => x.Region).HasMaxLength(120).IsRequired();
            b.Property<string>(x => x.City).HasMaxLength(120).IsRequired();
            b.Property<string>(x => x.Street).HasMaxLength(120).IsRequired();
            b.Property<string>(x => x.Building).HasMaxLength(32).IsRequired();
            b.Property<string?>(x => x.Apartment).HasMaxLength(32);
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.CitizenId).IsUnique();
            b.ToTable("Addresses");
        });

        modelBuilder.Entity<AuthorityOffice>(b =>
        {
            b.Property<Guid>(x => x.Id).ValueGeneratedOnAdd();
            b.Property<string>(x => x.Code).HasMaxLength(32).IsRequired();
            b.Property<string>(x => x.Name).HasMaxLength(180).IsRequired();
            b.Property<string>(x => x.Region).HasMaxLength(120).IsRequired();
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.Code).IsUnique();
            b.ToTable("AuthorityOffices");
        });

        modelBuilder.Entity<Employee>(b =>
        {
            b.Property<Guid>(x => x.Id).ValueGeneratedOnAdd();
            b.Property<Guid>(x => x.AuthorityOfficeId);
            b.Property<string>(x => x.PersonnelNumber).HasMaxLength(32).IsRequired();
            b.Property<string>(x => x.FullName).HasMaxLength(180).IsRequired();
            b.Property<string>(x => x.Role).HasMaxLength(64).IsRequired();
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.AuthorityOfficeId);
            b.HasIndex(x => x.PersonnelNumber).IsUnique();
            b.ToTable("Employees");
        });

        modelBuilder.Entity<Summons>(b =>
        {
            b.Property<Guid>(x => x.Id).ValueGeneratedOnAdd();
            b.Property<string>(x => x.Number).HasMaxLength(64).IsRequired();
            b.Property<Guid>(x => x.CitizenId);
            b.Property<Guid>(x => x.AuthorityOfficeId);
            b.Property<Guid>(x => x.CreatedByEmployeeId);
            b.Property<DateOnly>(x => x.IssuedAt);
            b.Property<DateTimeOffset>(x => x.DueAt);
            b.Property<string>(x => x.Reason).HasMaxLength(500).IsRequired();
            b.Property<SummonsStatus>(x => x.Status).HasConversion<string>();
            b.Property<string?>(x => x.Comment).HasMaxLength(1000);
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.Number).IsUnique();
            b.HasIndex(x => x.CitizenId);
            b.HasIndex(x => x.AuthorityOfficeId);
            b.HasIndex(x => x.CreatedByEmployeeId);
            b.ToTable("Summonses");
        });

        modelBuilder.Entity<SummonsStatusHistory>(b =>
        {
            b.Property<Guid>(x => x.Id).ValueGeneratedOnAdd();
            b.Property<Guid>(x => x.SummonsId);
            b.Property<SummonsStatus?>(x => x.FromStatus).HasConversion<string>();
            b.Property<SummonsStatus>(x => x.ToStatus).HasConversion<string>();
            b.Property<DateTimeOffset>(x => x.ChangedAt);
            b.Property<string>(x => x.ChangedBy).HasMaxLength(120).IsRequired();
            b.Property<string?>(x => x.Comment).HasMaxLength(500);
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.SummonsId);
            b.ToTable("SummonsStatusHistory");
        });

        modelBuilder.Entity<Notification>(b =>
        {
            b.Property<Guid>(x => x.Id).ValueGeneratedOnAdd();
            b.Property<Guid>(x => x.SummonsId);
            b.Property<NotificationChannel>(x => x.Channel).HasConversion<string>();
            b.Property<string>(x => x.DestinationMasked).HasMaxLength(255).IsRequired();
            b.Property<NotificationStatus>(x => x.Status).HasConversion<string>();
            b.Property<DateTimeOffset>(x => x.CreatedAt);
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.SummonsId);
            b.ToTable("Notifications");
        });

        modelBuilder.Entity<DeliveryAttempt>(b =>
        {
            b.Property<Guid>(x => x.Id).ValueGeneratedOnAdd();
            b.Property<Guid>(x => x.NotificationId);
            b.Property<int>(x => x.AttemptNumber);
            b.Property<string>(x => x.Result).HasMaxLength(64).IsRequired();
            b.Property<DateTimeOffset>(x => x.AttemptedAt);
            b.Property<string?>(x => x.ProviderMessage).HasMaxLength(500);
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.NotificationId);
            b.ToTable("DeliveryAttempts");
        });

        modelBuilder.Entity<Appeal>(b =>
        {
            b.Property<Guid>(x => x.Id).ValueGeneratedOnAdd();
            b.Property<Guid>(x => x.SummonsId);
            b.Property<string>(x => x.Type).HasMaxLength(64).IsRequired();
            b.Property<string>(x => x.Text).HasMaxLength(2000).IsRequired();
            b.Property<AppealStatus>(x => x.Status).HasConversion<string>();
            b.Property<DateTimeOffset>(x => x.SubmittedAt);
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.SummonsId);
            b.ToTable("Appeals");
        });

        modelBuilder.Entity<Document>(b =>
        {
            b.Property<Guid>(x => x.Id).ValueGeneratedOnAdd();
            b.Property<Guid>(x => x.SummonsId);
            b.Property<string>(x => x.FileName).HasMaxLength(255).IsRequired();
            b.Property<string>(x => x.MimeType).HasMaxLength(120).IsRequired();
            b.Property<string>(x => x.StorageUri).HasMaxLength(1000).IsRequired();
            b.Property<DateTimeOffset>(x => x.CreatedAt);
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.SummonsId);
            b.ToTable("Documents");
        });

        modelBuilder.Entity<AuditEvent>(b =>
        {
            b.Property<Guid>(x => x.Id).ValueGeneratedOnAdd();
            b.Property<Guid>(x => x.SummonsId);
            b.Property<string>(x => x.Action).HasMaxLength(100).IsRequired();
            b.Property<string>(x => x.Actor).HasMaxLength(120).IsRequired();
            b.Property<DateTimeOffset>(x => x.OccurredAt);
            b.Property<string?>(x => x.Details).HasMaxLength(2000);
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.SummonsId);
            b.ToTable("AuditEvents");
        });

        modelBuilder.Entity<SystemAccount>(b =>
        {
            b.Property<Guid>(x => x.Id).ValueGeneratedOnAdd();
            b.Property<string>(x => x.ExternalSubject).HasMaxLength(96).IsRequired();
            b.Property<string>(x => x.DisplayName).HasMaxLength(180).IsRequired();
            b.Property<string>(x => x.Role).HasMaxLength(64).IsRequired();
            b.Property<string?>(x => x.CitizenRegistryNumber).HasMaxLength(64);
            b.Property<bool>(x => x.IsActive);
            b.Property<DateTimeOffset>(x => x.CreatedAt);
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.ExternalSubject).IsUnique();
            b.ToTable("SystemAccounts");
        });

        modelBuilder.Entity<Address>()
            .HasOne(x => x.Citizen)
            .WithOne(x => x.Address)
            .HasForeignKey<Address>(x => x.CitizenId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Employee>()
            .HasOne(x => x.AuthorityOffice)
            .WithMany(x => x.Employees)
            .HasForeignKey(x => x.AuthorityOfficeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Summons>()
            .HasOne(x => x.Citizen)
            .WithMany(x => x.Summonses)
            .HasForeignKey(x => x.CitizenId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Summons>()
            .HasOne(x => x.AuthorityOffice)
            .WithMany(x => x.Summonses)
            .HasForeignKey(x => x.AuthorityOfficeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Summons>()
            .HasOne(x => x.CreatedByEmployee)
            .WithMany(x => x.CreatedSummonses)
            .HasForeignKey(x => x.CreatedByEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SummonsStatusHistory>()
            .HasOne(x => x.Summons)
            .WithMany(x => x.StatusHistory)
            .HasForeignKey(x => x.SummonsId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Notification>()
            .HasOne(x => x.Summons)
            .WithMany(x => x.Notifications)
            .HasForeignKey(x => x.SummonsId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DeliveryAttempt>()
            .HasOne(x => x.Notification)
            .WithMany(x => x.DeliveryAttempts)
            .HasForeignKey(x => x.NotificationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Appeal>()
            .HasOne(x => x.Summons)
            .WithMany(x => x.Appeals)
            .HasForeignKey(x => x.SummonsId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Document>()
            .HasOne(x => x.Summons)
            .WithMany(x => x.Documents)
            .HasForeignKey(x => x.SummonsId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AuditEvent>()
            .HasOne(x => x.Summons)
            .WithMany(x => x.AuditEvents)
            .HasForeignKey(x => x.SummonsId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
