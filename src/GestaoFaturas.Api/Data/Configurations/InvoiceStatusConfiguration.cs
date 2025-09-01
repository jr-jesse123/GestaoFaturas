using GestaoFaturas.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoFaturas.Api.Data.Configurations;

public class InvoiceStatusConfiguration : IEntityTypeConfiguration<InvoiceStatus>
{
    public void Configure(EntityTypeBuilder<InvoiceStatus> builder)
    {
        // Table configuration
        builder.ToTable("invoice_statuses");

        // Primary key
        builder.HasKey(s => s.Id);

        // Properties configuration
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.Description)
            .HasMaxLength(200);

        builder.Property(s => s.Color)
            .HasMaxLength(20);

        builder.Property(s => s.SortOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(s => s.IsActive)
            .HasDefaultValue(true);

        builder.Property(s => s.IsFinal)
            .HasDefaultValue(false);

        builder.Property(s => s.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(s => s.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Indexes
        builder.HasIndex(s => s.Name)
            .IsUnique()
            .HasDatabaseName("ix_invoice_statuses_name");

        builder.HasIndex(s => s.SortOrder)
            .HasDatabaseName("ix_invoice_statuses_sort_order");

        builder.HasIndex(s => s.IsActive)
            .HasDatabaseName("ix_invoice_statuses_is_active");

        // Relationships
        builder.HasMany(s => s.Invoices)
            .WithOne(i => i.InvoiceStatus)
            .HasForeignKey(i => i.InvoiceStatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}