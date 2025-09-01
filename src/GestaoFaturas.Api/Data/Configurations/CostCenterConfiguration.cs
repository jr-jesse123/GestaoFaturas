using GestaoFaturas.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoFaturas.Api.Data.Configurations;

public class CostCenterConfiguration : IEntityTypeConfiguration<CostCenter>
{
    public void Configure(EntityTypeBuilder<CostCenter> builder)
    {
        // Table configuration
        builder.ToTable("cost_centers");

        // Primary key
        builder.HasKey(cc => cc.Id);

        // Properties configuration
        builder.Property(cc => cc.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(cc => cc.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(cc => cc.Description)
            .HasMaxLength(500);

        builder.Property(cc => cc.IsActive)
            .HasDefaultValue(true);

        builder.Property(cc => cc.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(cc => cc.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Indexes
        builder.HasIndex(cc => new { cc.ClientId, cc.Code })
            .IsUnique()
            .HasDatabaseName("ix_cost_centers_client_code");

        builder.HasIndex(cc => cc.ParentCostCenterId)
            .HasDatabaseName("ix_cost_centers_parent_id");

        builder.HasIndex(cc => cc.IsActive)
            .HasDatabaseName("ix_cost_centers_is_active");

        // Relationships
        builder.HasOne(cc => cc.Client)
            .WithMany(c => c.CostCenters)
            .HasForeignKey(cc => cc.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        // Self-referencing relationship for hierarchical structure
        builder.HasOne(cc => cc.ParentCostCenter)
            .WithMany(cc => cc.ChildCostCenters)
            .HasForeignKey(cc => cc.ParentCostCenterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(cc => cc.ResponsiblePersons)
            .WithOne(rp => rp.CostCenter)
            .HasForeignKey(rp => rp.CostCenterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(cc => cc.Invoices)
            .WithOne(i => i.CostCenter)
            .HasForeignKey(i => i.CostCenterId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}