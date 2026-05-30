using MediatR;
using WLR.Application.DTOs;
namespace WLR.Application.Features.Motors.Commands.CloseMotor;
public record CloseMotorCommand(Guid MotorId, string? Notes) : IRequest<MotorLogDto>;
