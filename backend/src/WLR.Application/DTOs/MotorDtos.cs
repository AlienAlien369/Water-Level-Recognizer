using WLR.Domain.Enums;
namespace WLR.Application.DTOs;

public record MotorDto(
    Guid Id,
    Guid LocationId,
    string LocationName,
    Guid CenterId,
    string CenterName,
    string MotorNumber,
    string? Description,
    decimal WaterCapacityLiters,
    MotorStatus Status,
    MotorState CurrentState,
    DateTime? LastOpenedAt,
    DateTime? LastClosedAt,
    double TotalRunningMinutes,
    bool IsActive,
    Guid? AssignedSewadaarId,
    string? AssignedSewadaarName,
    DateTime CreatedAt
);

public record CreateMotorRequest(Guid LocationId, string MotorNumber, string? Description, decimal WaterCapacityLiters);
public record UpdateMotorRequest(string MotorNumber, string? Description, decimal WaterCapacityLiters);
public record OpenMotorRequest(string? Notes);
public record CloseMotorRequest(string? Notes);

public record MotorLogDto(
    Guid Id,
    Guid MotorId,
    string MotorNumber,
    Guid OperatedByUserId,
    string OperatedByUserName,
    string Action,
    DateTime ActionTime,
    double? DurationMinutes,
    string? Notes
);

public record MotorHistoryLogDto(
    Guid Id,
    Guid MotorId,
    string MotorNumber,
    string LocationName,
    string CenterName,
    Guid OperatedByUserId,
    string OperatedByUserName,
    string Action,
    DateTime ActionTime,
    double? DurationMinutes,
    string? Notes
);
