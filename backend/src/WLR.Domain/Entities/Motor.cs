using WLR.Domain.Common;
using WLR.Domain.Enums;
using WLR.Domain.Events;

namespace WLR.Domain.Entities;

public sealed class Motor : AuditableEntity
{
    public string MotorNumber { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal WaterCapacityLiters { get; private set; }
    public MotorStatus Status { get; private set; } = MotorStatus.Inactive;
    public MotorState CurrentState { get; private set; } = MotorState.Off;
    public DateTime? LastOpenedAt { get; private set; }
    public DateTime? LastClosedAt { get; private set; }
    public double TotalRunningMinutes { get; private set; } = 0;
    public bool IsActive { get; private set; } = true;
    public Guid LocationId { get; private set; }
    public Location? Location { get; private set; }
    public Guid? AssignedSewadaarId { get; private set; }
    public User? AssignedSewadaar { get; private set; }

    private readonly List<MotorLog> _logs = new();
    public IReadOnlyCollection<MotorLog> Logs => _logs.AsReadOnly();

    private Motor() { }

    public static Motor Create(Guid locationId, string motorNumber, string? description, decimal waterCapacityLiters)
    {
        return new Motor
        {
            LocationId = locationId,
            MotorNumber = motorNumber,
            Description = description,
            WaterCapacityLiters = waterCapacityLiters,
            Status = MotorStatus.Inactive
        };
    }

    public void Update(string motorNumber, string? description, decimal waterCapacityLiters)
    {
        MotorNumber = motorNumber;
        Description = description;
        WaterCapacityLiters = waterCapacityLiters;
        UpdatedAt = DateTime.UtcNow;
    }

    public MotorLog Open(Guid openedByUserId, string? notes = null)
    {
        if (CurrentState == MotorState.On)
            throw new InvalidOperationException("Motor is already running.");
        if (Status == MotorStatus.Fault || Status == MotorStatus.Maintenance)
            throw new InvalidOperationException("Motor cannot be started in its current status.");

        CurrentState = MotorState.On;
        Status = MotorStatus.Running;
        LastOpenedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        var log = MotorLog.CreateOpen(Id, openedByUserId, LastOpenedAt.Value, notes);
        _logs.Add(log);
        AddDomainEvent(new MotorOpenedEvent(Id, openedByUserId, LastOpenedAt.Value));
        return log;
    }

    public MotorLog Close(Guid closedByUserId, Guid openLogId, string? notes = null)
    {
        if (CurrentState == MotorState.Off)
            throw new InvalidOperationException("Motor is already stopped.");

        var duration = LastOpenedAt.HasValue ? (DateTime.UtcNow - LastOpenedAt.Value).TotalMinutes : 0;
        CurrentState = MotorState.Off;
        Status = MotorStatus.Active;
        LastClosedAt = DateTime.UtcNow;
        TotalRunningMinutes += duration;
        UpdatedAt = DateTime.UtcNow;

        var log = MotorLog.CreateClose(Id, closedByUserId, LastClosedAt.Value, duration, notes);
        _logs.Add(log);
        AddDomainEvent(new MotorClosedEvent(Id, closedByUserId, LastClosedAt.Value, duration));
        return log;
    }

    public void AssignSewadaar(Guid sewadaarId)
    {
        AssignedSewadaarId = sewadaarId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UnassignSewadaar()
    {
        AssignedSewadaarId = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetStatus(MotorStatus status)
    {
        Status = status;
        if (status == MotorStatus.Fault || status == MotorStatus.Maintenance)
            CurrentState = MotorState.Off;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate() { IsActive = true; Status = MotorStatus.Active; }
    public void Deactivate() { IsActive = false; Status = MotorStatus.Inactive; }
}
