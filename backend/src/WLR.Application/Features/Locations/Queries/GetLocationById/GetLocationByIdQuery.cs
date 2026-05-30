using MediatR;
using WLR.Application.DTOs;
namespace WLR.Application.Features.Locations.Queries.GetLocationById;
public record GetLocationByIdQuery(Guid Id) : IRequest<LocationDto>;
