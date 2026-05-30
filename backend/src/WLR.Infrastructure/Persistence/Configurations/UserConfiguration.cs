using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WLR.Domain.Entities;

namespace WLR.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Name).IsRequired().HasMaxLength(200);
        builder.Property(u => u.MobileNumber).IsRequired().HasMaxLength(20);
        builder.Property(u => u.Email).HasMaxLength(200);
        builder.Property(u => u.PasswordHash).HasMaxLength(500);
        builder.Property(u => u.ProfileImageUrl).HasMaxLength(500);
        builder.Property(u => u.Role).IsRequired();

        builder.HasIndex(u => u.MobileNumber).IsUnique();
        builder.HasIndex(u => u.Email);
        builder.HasIndex(u => u.Role);
        builder.HasIndex(u => u.IsActive);
        builder.HasIndex(u => u.CenterId);

        builder.HasMany(u => u.Assignments).WithOne(a => a.User).HasForeignKey(a => a.UserId);
        builder.HasMany(u => u.Sessions).WithOne(s => s.User).HasForeignKey(s => s.UserId);
    }
}
