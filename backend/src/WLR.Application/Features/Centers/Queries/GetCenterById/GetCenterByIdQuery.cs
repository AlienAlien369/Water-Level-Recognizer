using MediatR;
using WLR.Application.DTOs;
namespace WLR.Application.Features.Centers.Queries.GetCenterById;
public record GetCenterByIdQuery(Guid Id) : IRequest<CenterDto>;
