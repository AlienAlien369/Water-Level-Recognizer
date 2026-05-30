using MediatR;
using WLR.Application.DTOs;
namespace WLR.Application.Features.Locations.Commands.CreateLocation;
public record CreateLocationCommand(Guid CenterId, string Name, string? Description, string? Floor, string? Zone) : IRequest<LocationDto>;
