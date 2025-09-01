using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GestaoFaturas.Api.Models;
using GestaoFaturas.Api.Data.Configurations;
using System.Reflection;

using GestaoFaturas.Api.Models;
using GestaoFaturas.Api.Data.Configurations;
using System.Reflection;


namespace GestaoFaturas.Api.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }


    // DbSets for domain entities
    public DbSet<Client> Clients { get; set; }
    public DbSet<CostCenter> CostCenters { get; set; }
    public DbSet<ResponsiblePerson> ResponsiblePersons { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceStatus> InvoiceStatuses { get; set; }
    public DbSet<InvoiceHistory> InvoiceHistories { get; set; }


    // DbSets for domain entities
    public DbSet<Client> Clients { get; set; }
    public DbSet<CostCenter> CostCenters { get; set; }
    public DbSet<ResponsiblePerson> ResponsiblePersons { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceStatus> InvoiceStatuses { get; set; }
    public DbSet<InvoiceHistory> InvoiceHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure PostgreSQL naming conventions using snake_case
        // This will be handled by the EFCore.NamingConventions package

        // Apply all entity configurations
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Configure audit fields to be automatically set
        ConfigureAuditFields(modelBuilder);
    }

    private static void ConfigureAuditFields(ModelBuilder modelBuilder)
    {
        // Configure audit fields for all entities
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Set CreatedAt and UpdatedAt properties to use UTC
            if (entityType.FindProperty("CreatedAt") != null)
            {
                entityType.FindProperty("CreatedAt")!.SetValueConverter(new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc)));
            }

            if (entityType.FindProperty("UpdatedAt") != null)
            {
                entityType.FindProperty("UpdatedAt")!.SetValueConverter(new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc)));
            }
        }
    }

    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditFields()
    {
        var entities = ChangeTracker.Entries()
            .Where(x => x.Entity is Client || x.Entity is CostCenter || x.Entity is ResponsiblePerson ||
                       x.Entity is Invoice || x.Entity is InvoiceStatus || x.Entity is InvoiceHistory)
            .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);

        foreach (var entity in entities)
        {
            var now = DateTime.UtcNow;

            if (entity.State == EntityState.Added)
            {
                // Check if entity has CreatedAt property before trying to set it
                var createdAtProperty = entity.Properties.FirstOrDefault(p => p.Metadata.Name == "CreatedAt");
                if (createdAtProperty != null)
                {
                    if (createdAtProperty.CurrentValue == null ||
                        (createdAtProperty.CurrentValue is DateTime createdAt && createdAt == DateTime.MinValue))
                        createdAtProperty.CurrentValue = now;
                }
            }

            // Check if entity has UpdatedAt property before trying to set it
            var updatedAtProperty = entity.Properties.FirstOrDefault(p => p.Metadata.Name == "UpdatedAt");
            if (updatedAtProperty != null)
            {
                updatedAtProperty.CurrentValue = now;
            }
        }

    }
}
