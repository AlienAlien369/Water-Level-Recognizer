using MediatR;
using WLR.Application.DTOs;
namespace WLR.Application.Features.Users.Commands.UpdateUserProfile;
public record UpdateUserProfileCommand(Guid UserId, string Name, string? Email, string? ProfileImageUrl) : IRequest<UserDto>;
