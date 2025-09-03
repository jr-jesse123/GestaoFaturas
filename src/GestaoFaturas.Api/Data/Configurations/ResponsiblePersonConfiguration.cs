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
        builder.Property(rp => rp.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(rp => rp.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(rp => rp.Phone)
            .HasMaxLength(20);

        builder.Property(rp => rp.Role)
            .HasMaxLength(100);

        builder.Property(rp => rp.Department)
            .HasMaxLength(50);

        builder.Property(rp => rp.IsActive)
            .HasDefaultValue(true);

        builder.Property(rp => rp.ReceivesNotifications)
            .HasDefaultValue(true);

        builder.Property(rp => rp.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(rp => rp.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Indexes
        builder.HasIndex(rp => rp.Email)
            .HasDatabaseName("ix_responsible_persons_email")
            .IsUnique();

        builder.HasIndex(rp => rp.ClientId)
            .HasDatabaseName("ix_responsible_persons_client_id");

        builder.HasIndex(rp => rp.IsActive)
            .HasDatabaseName("ix_responsible_persons_is_active");

        // Relationships
        builder.HasOne(rp => rp.Client)
            .WithMany(c => c.ResponsiblePersons)
            .HasForeignKey(rp => rp.ClientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}