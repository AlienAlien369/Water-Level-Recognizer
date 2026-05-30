using WLR.Domain.Common;

namespace WLR.Domain.Entities;

public sealed class MotorLog : BaseEntity
{
    public Guid MotorId { get; private set; }
    public Motor? Motor { get; private set; }
    public Guid OperatedByUserId { get; private set; }
    public User? OperatedByUser { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public DateTime ActionTime { get; private set; }
    public double? DurationMinutes { get; private set; }
    public string? Notes { get; private set; }

    private MotorLog() { }

    public static MotorLog CreateOpen(Guid motorId, Guid userId, DateTime openTime, string? notes)
    {
        return new MotorLog
        {
            MotorId = motorId,
            OperatedByUserId = userId,
            Action = "Open",
            ActionTime = openTime,
            Notes = notes
        };
    }

    public static MotorLog CreateClose(Guid motorId, Guid userId, DateTime closeTime, double durationMinutes, string? notes)
    {
        return new MotorLog
        {
            MotorId = motorId,
            OperatedByUserId = userId,
            Action = "Close",
            ActionTime = closeTime,
            DurationMinutes = durationMinutes,
            Notes = notes
        };
    }
}
