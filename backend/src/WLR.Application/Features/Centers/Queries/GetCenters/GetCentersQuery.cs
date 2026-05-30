using MediatR;
using WLR.Application.Common.Models;
using WLR.Application.DTOs;
namespace WLR.Application.Features.Centers.Queries.GetCenters;
public record GetCentersQuery(QueryParams Params) : IRequest<PaginatedResult<CenterDto>>;
