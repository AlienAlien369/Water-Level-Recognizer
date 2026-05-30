using WLR.Domain.Enums;
namespace WLR.Application.DTOs;

public record DashboardSummaryDto(
    int TotalCenters,
    int TotalLocations,
    int TotalMotors,
    int TotalUsers,
    int ActiveMotors,
    int RunningMotors,
    int FaultMotors,
    int TotalSewadars,
    double TotalRunningMinutesToday
);

public record MotorStatusSummaryDto(
    Guid MotorId,
    string MotorNumber,
    string LocationName,
    string CenterName,
    MotorState CurrentState,
    MotorStatus Status,
    DateTime? LastOpenedAt,
    double? CurrentRunningMinutes,
    string? AssignedSewadaarName
);

public record CenterAnalyticsDto(
    Guid CenterId,
    string CenterName,
    int TotalMotors,
    int RunningMotors,
    int FaultMotors,
    double TotalRunningMinutesToday,
    double CapacityUtilizationPercent
);
