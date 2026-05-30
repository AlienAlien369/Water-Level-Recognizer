using MediatR;
using WLR.Application.DTOs;
namespace WLR.Application.Features.Auth.Commands.Login;
public record LoginCommand(string MobileNumber, string OtpCode, string? DeviceInfo, string? IpAddress, string? UserAgent) : IRequest<AuthResponse>;
