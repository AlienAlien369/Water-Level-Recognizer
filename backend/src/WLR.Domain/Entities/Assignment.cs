using WLR.Domain.Common;
using WLR.Domain.Enums;

namespace WLR.Domain.Entities;

public sealed class Assignment : AuditableEntity
{
    public Guid UserId { get; private set; }
    public User? User { get; private set; }
    public Guid? LocationId { get; private set; }
    public Location? Location { get; private set; }
    public Guid? MotorId { get; private set; }
    public Motor? Motor { get; private set; }
    public Guid CenterId { get; private set; }
    public Center? Center { get; private set; }
    public AssignmentStatus Status { get; private set; } = AssignmentStatus.Active;
    public DateTime AssignedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? RevokedAt { get; private set; }
    public string? Notes { get; private set; }

    private Assignment() { }

    public static Assignment Create(Guid userId, Guid centerId, Guid? locationId, Guid? motorId, string? notes = null)
    {
        return new Assignment
        {
            UserId = userId,
            CenterId = centerId,
            LocationId = locationId,
            MotorId = motorId,
            Notes = notes,
            Status = AssignmentStatus.Active
        };
    }

    public void Revoke()
    {
        Status = AssignmentStatus.Revoked;
        RevokedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        Status = AssignmentStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
    }
}
