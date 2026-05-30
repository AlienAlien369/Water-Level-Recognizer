using MediatR;
namespace WLR.Application.Features.Users.Commands.ToggleUserActive;
public record ToggleUserActiveCommand(Guid UserId, bool Activate) : IRequest<bool>;
