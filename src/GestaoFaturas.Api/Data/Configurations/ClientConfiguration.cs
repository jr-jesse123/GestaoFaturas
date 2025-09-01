using GestaoFaturas.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoFaturas.Api.Data.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        // Table configuration
        builder.ToTable("clients");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties configuration
        builder.Property(c => c.CompanyName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.TradeName)
            .HasMaxLength(200);

        builder.Property(c => c.TaxId)
            .IsRequired()
            .HasMaxLength(14);

        builder.Property(c => c.Email)
            .HasMaxLength(100);

        builder.Property(c => c.Phone)
            .HasMaxLength(20);

        builder.Property(c => c.Address)
            .HasMaxLength(500);

        builder.Property(c => c.ContactPerson)
            .HasMaxLength(100);

        builder.Property(c => c.IsActive)
            .HasDefaultValue(true);

        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(c => c.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Indexes
        builder.HasIndex(c => c.TaxId)
            .IsUnique()
            .HasDatabaseName("ix_clients_tax_id");

        builder.HasIndex(c => c.Email)
            .HasDatabaseName("ix_clients_email");

        builder.HasIndex(c => c.CompanyName)
            .HasDatabaseName("ix_clients_company_name");

        builder.HasIndex(c => c.IsActive)
            .HasDatabaseName("ix_clients_is_active");

        // Relationships
        builder.HasMany(c => c.CostCenters)
            .WithOne(cc => cc.Client)
            .HasForeignKey(cc => cc.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Invoices)
            .WithOne(i => i.Client)
            .HasForeignKey(i => i.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}