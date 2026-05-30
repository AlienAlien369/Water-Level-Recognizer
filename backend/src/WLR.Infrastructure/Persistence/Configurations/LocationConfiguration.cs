using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WLR.Domain.Entities;

namespace WLR.Infrastructure.Persistence.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("Locations");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Name).IsRequired().HasMaxLength(200);
        builder.Property(l => l.Description).HasMaxLength(1000);
        builder.Property(l => l.Floor).HasMaxLength(50);
        builder.Property(l => l.Zone).HasMaxLength(100);
        builder.HasIndex(l => l.CenterId);
        builder.HasIndex(l => l.IsActive);
        builder.HasMany(l => l.Motors).WithOne(m => m.Location).HasForeignKey(m => m.LocationId);
    }
}
