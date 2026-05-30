using MediatR;
using WLR.Application.DTOs;
namespace WLR.Application.Features.Motors.Commands.OpenMotor;
public record OpenMotorCommand(Guid MotorId, string? Notes) : IRequest<MotorLogDto>;
