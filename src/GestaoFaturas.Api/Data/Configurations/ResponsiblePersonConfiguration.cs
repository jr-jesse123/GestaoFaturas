using GestaoFaturas.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoFaturas.Api.Data.Configurations;

public class ResponsiblePersonConfiguration : IEntityTypeConfiguration<ResponsiblePerson>
{
    public void Configure(EntityTypeBuilder<ResponsiblePerson> builder)
    {
        // Table configuration
        builder.ToTable("responsible_persons");

        // Primary key
        builder.HasKey(rp => rp.Id);

        // Properties configuration
        builder.Property(rp => rp.FullName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(rp => rp.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(rp => rp.Phone)
            .HasMaxLength(20);

        builder.Property(rp => rp.Position)
            .HasMaxLength(100);

        builder.Property(rp => rp.Department)
            .HasMaxLength(50);

        builder.Property(rp => rp.IsActive)
            .HasDefaultValue(true);

        builder.Property(rp => rp.IsPrimary)
            .HasDefaultValue(false);

        builder.Property(rp => rp.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(rp => rp.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Indexes
        builder.HasIndex(rp => rp.Email)
            .HasDatabaseName("ix_responsible_persons_email");

        builder.HasIndex(rp => new { rp.CostCenterId, rp.IsPrimary })
            .HasDatabaseName("ix_responsible_persons_cost_center_primary");

        builder.HasIndex(rp => rp.IsActive)
            .HasDatabaseName("ix_responsible_persons_is_active");

        // Relationships
        builder.HasOne(rp => rp.CostCenter)
            .WithMany(cc => cc.ResponsiblePersons)
            .HasForeignKey(rp => rp.CostCenterId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}