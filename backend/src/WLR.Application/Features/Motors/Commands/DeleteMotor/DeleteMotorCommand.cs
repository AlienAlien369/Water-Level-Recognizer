using MediatR;
namespace WLR.Application.Features.Motors.Commands.DeleteMotor;
public record DeleteMotorCommand(Guid Id) : IRequest<bool>;
