using WLR.Domain.Enums;
namespace WLR.Application.DTOs;

public record AssignmentDto(
    Guid Id,
    Guid UserId,
    string UserName,
    string UserMobile,
    Guid CenterId,
    string CenterName,
    Guid? LocationId,
    string? LocationName,
    Guid? MotorId,
    string? MotorNumber,
    AssignmentStatus Status,
    DateTime AssignedAt,
    DateTime? RevokedAt,
    string? Notes,
    DateTime CreatedAt
);

public record CreateAssignmentRequest(
    Guid UserId,
    Guid CenterId,
    Guid? LocationId,
    Guid? MotorId,
    string? Notes
);
