using MediatR;
using WLR.Application.DTOs;
namespace WLR.Application.Features.Motors.Queries.GetMotorById;
public record GetMotorByIdQuery(Guid Id) : IRequest<MotorDto>;
