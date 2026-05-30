using MediatR;
using WLR.Application.Common.Models;
using WLR.Application.DTOs;
namespace WLR.Application.Features.Locations.Queries.GetLocations;
public record GetLocationsQuery(QueryParams Params, Guid? CenterId) : IRequest<PaginatedResult<LocationDto>>;
