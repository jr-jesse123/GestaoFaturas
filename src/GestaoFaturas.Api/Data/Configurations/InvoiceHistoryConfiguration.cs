using GestaoFaturas.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoFaturas.Api.Data.Configurations;

public class InvoiceHistoryConfiguration : IEntityTypeConfiguration<InvoiceHistory>
{
    public void Configure(EntityTypeBuilder<InvoiceHistory> builder)
    {
        // Table configuration
        builder.ToTable("invoice_histories");

        // Primary key
        builder.HasKey(ih => ih.Id);

        // Properties configuration
        builder.Property(ih => ih.ChangeReason)
            .HasMaxLength(500);

        builder.Property(ih => ih.Comments)
            .HasMaxLength(1000);

        builder.Property(ih => ih.ChangedByUserId)
            .HasMaxLength(100);

        builder.Property(ih => ih.ChangedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Indexes
        builder.HasIndex(ih => ih.InvoiceId)
            .HasDatabaseName("ix_invoice_histories_invoice_id");

        builder.HasIndex(ih => ih.ChangedAt)
            .HasDatabaseName("ix_invoice_histories_changed_at");

        builder.HasIndex(ih => ih.ChangedByUserId)
            .HasDatabaseName("ix_invoice_histories_changed_by");

        builder.HasIndex(ih => new { ih.FromStatusId, ih.ToStatusId })
            .HasDatabaseName("ix_invoice_histories_status_transition");

        // Relationships
        builder.HasOne(ih => ih.Invoice)
            .WithMany(i => i.InvoiceHistories)
            .HasForeignKey(ih => ih.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ih => ih.FromStatus)
            .WithMany()
            .HasForeignKey(ih => ih.FromStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ih => ih.ToStatus)
            .WithMany()
            .HasForeignKey(ih => ih.ToStatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}