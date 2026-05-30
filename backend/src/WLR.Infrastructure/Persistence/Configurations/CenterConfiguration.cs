using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WLR.Domain.Entities;

namespace WLR.Infrastructure.Persistence.Configurations;

public class CenterConfiguration : IEntityTypeConfiguration<Center>
{
    public void Configure(EntityTypeBuilder<Center> builder)
    {
        builder.ToTable("Centers");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Description).HasMaxLength(1000);
        builder.Property(c => c.Address).HasMaxLength(500);
        builder.Property(c => c.City).HasMaxLength(100);
        builder.Property(c => c.State).HasMaxLength(100);
        builder.Property(c => c.Country).HasMaxLength(100);
        builder.Property(c => c.ContactPhone).HasMaxLength(20);
        builder.Property(c => c.ContactEmail).HasMaxLength(200);
        builder.HasIndex(c => c.Name);
        builder.HasIndex(c => c.IsActive);
        builder.HasMany(c => c.Locations).WithOne(l => l.Center).HasForeignKey(l => l.CenterId);
    }
}
