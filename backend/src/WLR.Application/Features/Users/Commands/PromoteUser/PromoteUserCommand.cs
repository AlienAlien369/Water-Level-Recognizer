using MediatR;
using WLR.Application.DTOs;
namespace WLR.Application.Features.Users.Commands.PromoteUser;
public record PromoteUserCommand(Guid UserId, Guid CenterId) : IRequest<UserDto>;
