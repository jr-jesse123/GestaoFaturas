using GestaoFaturas.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoFaturas.Api.Data.Configurations;

public class CostCenterResponsibleConfiguration : IEntityTypeConfiguration<CostCenterResponsible>
{
    public void Configure(EntityTypeBuilder<CostCenterResponsible> builder)
    {
        // Table configuration
        builder.ToTable("cost_center_responsibles");

        // Composite primary key
        builder.HasKey(ccr => new { ccr.CostCenterId, ccr.ResponsiblePersonId });

        // Properties configuration
        builder.Property(ccr => ccr.AssignedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(ccr => ccr.IsPrimary)
            .HasDefaultValue(false);

        // Indexes
        builder.HasIndex(ccr => new { ccr.CostCenterId, ccr.IsPrimary })
            .HasDatabaseName("ix_cost_center_responsibles_cost_center_primary")
            .HasFilter("is_primary = true")
            .IsUnique();

        // Relationships
        builder.HasOne(ccr => ccr.CostCenter)
            .WithMany(cc => cc.CostCenterResponsibles)
            .HasForeignKey(ccr => ccr.CostCenterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ccr => ccr.ResponsiblePerson)
            .WithMany(rp => rp.CostCenterResponsibles)
            .HasForeignKey(ccr => ccr.ResponsiblePersonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}