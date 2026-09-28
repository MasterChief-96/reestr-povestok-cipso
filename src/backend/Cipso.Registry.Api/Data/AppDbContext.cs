using Cipso.Registry.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Cipso.Registry.Api.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Citizen> Citizens => Set<Citizen>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<AuthorityOffice> AuthorityOffices => Set<AuthorityOffice>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Summons> Summonses => Set<Summons>();
    public DbSet<SummonsStatusHistory> SummonsStatusHistory => Set<SummonsStatusHistory>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<DeliveryAttempt> DeliveryAttempts => Set<DeliveryAttempt>();
    public DbSet<Appeal> Appeals => Set<Appeal>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();
    public DbSet<SystemAccount> SystemAccounts => Set<SystemAccount>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Citizen>().HasIndex(x => x.RegistryNumber).IsUnique();
        modelBuilder.Entity<AuthorityOffice>().HasIndex(x => x.Code).IsUnique();
        modelBuilder.Entity<Employee>().HasIndex(x => x.PersonnelNumber).IsUnique();
        modelBuilder.Entity<Summons>().HasIndex(x => x.Number).IsUnique();
        modelBuilder.Entity<SystemAccount>().HasIndex(x => x.ExternalSubject).IsUnique();

        modelBuilder.Entity<Citizen>()
            .HasOne(x => x.Address)
            .WithOne(x => x.Citizen)
            .HasForeignKey<Address>(x => x.CitizenId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Employee>()
            .HasOne(x => x.AuthorityOffice)
            .WithMany(x => x.Employees)
            .HasForeignKey(x => x.AuthorityOfficeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Summons>()
            .HasOne(x => x.CreatedByEmployee)
            .WithMany(x => x.CreatedSummonses)
            .HasForeignKey(x => x.CreatedByEmployeeId)
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

        modelBuilder.Entity<Summons>().Property(x => x.Status).HasConversion<string>();
        modelBuilder.Entity<SummonsStatusHistory>().Property(x => x.FromStatus).HasConversion<string>();
        modelBuilder.Entity<SummonsStatusHistory>().Property(x => x.ToStatus).HasConversion<string>();
        modelBuilder.Entity<Notification>().Property(x => x.Channel).HasConversion<string>();
        modelBuilder.Entity<Notification>().Property(x => x.Status).HasConversion<string>();
        modelBuilder.Entity<Appeal>().Property(x => x.Status).HasConversion<string>();
    }
}
