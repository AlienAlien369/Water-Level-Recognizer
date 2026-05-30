using MediatR;
using WLR.Application.DTOs;
namespace WLR.Application.Features.Locations.Commands.UpdateLocation;
public record UpdateLocationCommand(Guid Id, string Name, string? Description, string? Floor, string? Zone) : IRequest<LocationDto>;
