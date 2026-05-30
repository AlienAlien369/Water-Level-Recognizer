using MediatR;
using WLR.Application.DTOs;
namespace WLR.Application.Features.Auth.Commands.RefreshToken;
public record RefreshTokenCommand(string RefreshToken, string? IpAddress) : IRequest<AuthResponse>;
