using MediatR;
namespace WLR.Application.Features.Centers.Commands.DeleteCenter;
public record DeleteCenterCommand(Guid Id) : IRequest<bool>;
