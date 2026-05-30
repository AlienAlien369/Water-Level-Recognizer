using MediatR;
using WLR.Application.DTOs;
namespace WLR.Application.Features.Motors.Commands.UpdateMotor;
public record UpdateMotorCommand(Guid Id, string MotorNumber, string? Description, decimal WaterCapacityLiters) : IRequest<MotorDto>;
