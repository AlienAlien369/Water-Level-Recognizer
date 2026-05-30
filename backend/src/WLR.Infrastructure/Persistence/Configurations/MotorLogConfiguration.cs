using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WLR.Domain.Entities;

namespace WLR.Infrastructure.Persistence.Configurations;

public class MotorLogConfiguration : IEntityTypeConfiguration<MotorLog>
{
    public void Configure(EntityTypeBuilder<MotorLog> builder)
    {
        builder.ToTable("MotorLogs");
        builder.HasKey(ml => ml.Id);
        builder.Property(ml => ml.Action).IsRequired().HasMaxLength(10);
        builder.Property(ml => ml.Notes).HasMaxLength(500);
        builder.HasIndex(ml => ml.MotorId);
        builder.HasIndex(ml => ml.ActionTime);
        builder.HasIndex(ml => ml.OperatedByUserId);
        builder.HasOne(ml => ml.OperatedByUser).WithMany().HasForeignKey(ml => ml.OperatedByUserId);
    }
}
