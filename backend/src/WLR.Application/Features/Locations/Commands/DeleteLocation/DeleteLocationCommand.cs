using MediatR;
namespace WLR.Application.Features.Locations.Commands.DeleteLocation;
public record DeleteLocationCommand(Guid Id) : IRequest<bool>;
