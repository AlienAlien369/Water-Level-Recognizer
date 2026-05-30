using Microsoft.EntityFrameworkCore;
using System.Reflection;
using WLR.Application.Common.Interfaces;
using WLR.Domain.Common;
using WLR.Domain.Entities;

namespace WLR.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly ICurrentUser _currentUser;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUser currentUser)
        : base(options)
    {
        _currentUser = currentUser;
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Center> Centers => Set<Center>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Motor> Motors => Set<Motor>();
    public DbSet<MotorLog> MotorLogs => Set<MotorLog>();
    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();
    public DbSet<OtpVerification> OtpVerifications => Set<OtpVerification>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);

        // Global query filter for soft deletes
        builder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Center>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Location>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Motor>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Assignment>().HasQueryFilter(e => !e.IsDeleted);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    if (_currentUser.UserId.HasValue)
                        entry.Entity.SetCreatedBy(_currentUser.UserId.Value.ToString());
                    break;
                case EntityState.Modified:
                    if (_currentUser.UserId.HasValue)
                        entry.Entity.SetUpdatedBy(_currentUser.UserId.Value.ToString());
                    break;
            }
        }

        var result = await base.SaveChangesAsync(cancellationToken);

        // Dispatch domain events
        var domainEvents = ChangeTracker.Entries<BaseEntity>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .SelectMany(e => e.DomainEvents)
            .ToList();

        foreach (var entity in ChangeTracker.Entries<BaseEntity>().Select(e => e.Entity))
            entity.ClearDomainEvents();

        return result;
    }
}
