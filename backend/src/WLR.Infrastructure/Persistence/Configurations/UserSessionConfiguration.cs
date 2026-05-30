using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WLR.Domain.Entities;

namespace WLR.Infrastructure.Persistence.Configurations;

public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable("UserSessions");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.RefreshToken).IsRequired().HasMaxLength(500);
        builder.Property(s => s.DeviceInfo).HasMaxLength(500);
        builder.Property(s => s.IpAddress).HasMaxLength(50);
        builder.Property(s => s.UserAgent).HasMaxLength(500);
        builder.HasIndex(s => s.RefreshToken).IsUnique();
        builder.HasIndex(s => s.UserId);
        builder.HasIndex(s => s.ExpiresAt);
    }
}
