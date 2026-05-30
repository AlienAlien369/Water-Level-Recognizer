using MediatR;
using WLR.Application.DTOs;
namespace WLR.Application.Features.Motors.Commands.CreateMotor;
public record CreateMotorCommand(Guid LocationId, string MotorNumber, string? Description, decimal WaterCapacityLiters) : IRequest<MotorDto>;
