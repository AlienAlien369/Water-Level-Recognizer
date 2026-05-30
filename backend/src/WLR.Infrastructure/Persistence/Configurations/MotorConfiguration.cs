using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WLR.Domain.Entities;

namespace WLR.Infrastructure.Persistence.Configurations;

public class MotorConfiguration : IEntityTypeConfiguration<Motor>
{
    public void Configure(EntityTypeBuilder<Motor> builder)
    {
        builder.ToTable("Motors");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.MotorNumber).IsRequired().HasMaxLength(100);
        builder.Property(m => m.Description).HasMaxLength(500);
        builder.Property(m => m.WaterCapacityLiters).HasColumnType("decimal(18,2)");
        builder.HasIndex(m => m.LocationId);
        builder.HasIndex(m => m.Status);
        builder.HasIndex(m => m.CurrentState);
        builder.HasIndex(m => m.AssignedSewadaarId);
        builder.HasIndex(m => new { m.LocationId, m.MotorNumber }).IsUnique();
        builder.HasOne(m => m.AssignedSewadaar).WithMany().HasForeignKey(m => m.AssignedSewadaarId).IsRequired(false);
        builder.HasMany(m => m.Logs).WithOne(ml => ml.Motor).HasForeignKey(ml => ml.MotorId);
    }
}
