namespace WLR.Application.DTOs;

public record CenterDto(
    Guid Id,
    string Name,
    string? Description,
    string? Address,
    string? City,
    string? State,
    string? Country,
    string? ContactPhone,
    string? ContactEmail,
    bool IsActive,
    int LocationCount,
    int MotorCount,
    DateTime CreatedAt,
    bool RequiresAssignment = true
);

public record CreateCenterRequest(
    string Name,
    string? Description,
    string? Address,
    string? City,
    string? State,
    string? Country,
    string? ContactPhone,
    string? ContactEmail
);

public record UpdateCenterRequest(
    string Name,
    string? Description,
    string? Address,
    string? City,
    string? State,
    string? Country,
    string? ContactPhone,
    string? ContactEmail
);

public record ToggleCenterAssignmentRequest(bool RequiresAssignment);
