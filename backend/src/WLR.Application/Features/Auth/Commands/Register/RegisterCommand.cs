using MediatR;
using WLR.Application.DTOs;
namespace WLR.Application.Features.Auth.Commands.Register;
public record RegisterCommand(string Name, string MobileNumber, string OtpCode, string? Email, string? DeviceInfo, string? IpAddress, string? UserAgent) : IRequest<AuthResponse>;
