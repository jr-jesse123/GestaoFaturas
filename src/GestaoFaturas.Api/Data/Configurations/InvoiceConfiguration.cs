using GestaoFaturas.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoFaturas.Api.Data.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        // Table configuration
        builder.ToTable("invoices");

        // Primary key
        builder.HasKey(i => i.Id);

        // Properties configuration
        builder.Property(i => i.InvoiceNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(i => i.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.TaxAmount)
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.TotalAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.IssueDate)
            .IsRequired();

        builder.Property(i => i.DueDate)
            .IsRequired();

        builder.Property(i => i.ServicePeriodStart)
            .IsRequired();

        builder.Property(i => i.ServicePeriodEnd)
            .IsRequired();

        builder.Property(i => i.ServiceType)
            .HasMaxLength(50);

        builder.Property(i => i.Description)
            .HasMaxLength(1000);

        builder.Property(i => i.DocumentPath)
            .HasMaxLength(500);

        builder.Property(i => i.Notes)
            .HasMaxLength(1000);

        builder.Property(i => i.IsActive)
            .HasDefaultValue(true);

        builder.Property(i => i.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(i => i.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Check constraints using modern EF Core syntax
        builder.ToTable(t => 
        {
            t.HasCheckConstraint("CK_Invoices_Amount_Positive", "amount > 0");
            t.HasCheckConstraint("CK_Invoices_TotalAmount_Positive", "total_amount > 0");
            t.HasCheckConstraint("CK_Invoices_ServicePeriod_Valid", "service_period_start <= service_period_end");
            t.HasCheckConstraint("CK_Invoices_DueDate_Valid", "issue_date <= due_date");
        });

        // Indexes
        builder.HasIndex(i => new { i.ClientId, i.InvoiceNumber })
            .IsUnique()
            .HasDatabaseName("ix_invoices_client_number");

        builder.HasIndex(i => new { i.ClientId, i.CostCenterId, i.IssueDate })
            .HasDatabaseName("ix_invoices_client_cost_center_date");

        builder.HasIndex(i => i.InvoiceStatusId)
            .HasDatabaseName("ix_invoices_status");

        builder.HasIndex(i => i.DueDate)
            .HasDatabaseName("ix_invoices_due_date");

        builder.HasIndex(i => i.IssueDate)
            .HasDatabaseName("ix_invoices_issue_date");

        builder.HasIndex(i => new { i.ServicePeriodStart, i.ServicePeriodEnd })
            .HasDatabaseName("ix_invoices_service_period");

        builder.HasIndex(i => i.IsActive)
            .HasDatabaseName("ix_invoices_is_active");

        // Relationships
        builder.HasOne(i => i.Client)
            .WithMany(c => c.Invoices)
            .HasForeignKey(i => i.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.CostCenter)
            .WithMany(cc => cc.Invoices)
            .HasForeignKey(i => i.CostCenterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.InvoiceStatus)
            .WithMany(s => s.Invoices)
            .HasForeignKey(i => i.InvoiceStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(i => i.InvoiceHistories)
            .WithOne(ih => ih.Invoice)
            .HasForeignKey(ih => ih.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}