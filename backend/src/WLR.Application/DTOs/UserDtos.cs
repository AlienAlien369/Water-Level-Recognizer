namespace WLR.Application.DTOs;

public record UpdateUserProfileRequest(string Name, string? Email, string? ProfileImageUrl);
public record PromoteUserRequest(Guid CenterId);
