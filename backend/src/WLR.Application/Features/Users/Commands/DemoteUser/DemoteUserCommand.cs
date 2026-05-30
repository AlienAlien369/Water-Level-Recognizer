using MediatR;
using WLR.Application.DTOs;
namespace WLR.Application.Features.Users.Commands.DemoteUser;
public record DemoteUserCommand(Guid UserId) : IRequest<UserDto>;
