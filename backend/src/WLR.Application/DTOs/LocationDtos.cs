namespace WLR.Application.DTOs;

public record LocationDto(
    Guid Id,
    Guid CenterId,
    string CenterName,
    string Name,
    string? Description,
    string? Floor,
    string? Zone,
    bool IsActive,
    int MotorCount,
    int ActiveMotorCount,
    DateTime CreatedAt
);

public record CreateLocationRequest(Guid CenterId, string Name, string? Description, string? Floor, string? Zone);
public record UpdateLocationRequest(string Name, string? Description, string? Floor, string? Zone);
