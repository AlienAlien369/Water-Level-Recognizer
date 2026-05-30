using Microsoft.EntityFrameworkCore;
using WLR.Domain.Entities;
namespace WLR.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Center> Centers { get; }
    DbSet<Location> Locations { get; }
    DbSet<Motor> Motors { get; }
    DbSet<MotorLog> MotorLogs { get; }
    DbSet<Assignment> Assignments { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<AuditLog> AuditLogs { get; }
    DbSet<UserSession> UserSessions { get; }
    DbSet<OtpVerification> OtpVerifications { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
