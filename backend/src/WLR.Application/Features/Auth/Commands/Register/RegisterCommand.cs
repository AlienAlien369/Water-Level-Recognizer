using MediatR;
using WLR.Application.DTOs;
namespace WLR.Application.Features.Auth.Commands.Register;
public record RegisterCommand(string Name, string MobileNumber, string Password, string? Email, string? DeviceInfo, string? IpAddress, string? UserAgent, Guid? CenterId = null) : IRequest<AuthResponse>;
