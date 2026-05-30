using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WLR.Application.DTOs;
using WLR.Application.Features.Auth.Commands.Login;
using WLR.Application.Features.Auth.Commands.Register;
using WLR.Application.Features.Auth.Commands.RefreshToken;
using WLR.Application.Features.Auth.Commands.SendOtp;
using WLR.Application.Common.Models;

namespace WLR.API.Controllers.V1;

[EnableRateLimiting("auth")]
public class AuthController : BaseController
{
    [HttpPost("send-otp")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new SendOtpCommand(request.MobileNumber), cancellationToken);
        return Ok(ApiResponse<bool>.Ok(result, "OTP sent successfully."));
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), 200)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = Request.Headers["User-Agent"].ToString();
        var result = await Mediator.Send(new RegisterCommand(request.Name, request.MobileNumber, request.OtpCode, request.Email, null, ip, userAgent), cancellationToken);
        return Ok(ApiResponse<AuthResponse>.Ok(result, "Registration successful."));
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), 200)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = Request.Headers["User-Agent"].ToString();
        var result = await Mediator.Send(new LoginCommand(request.MobileNumber, request.OtpCode, null, ip, userAgent), cancellationToken);
        return Ok(ApiResponse<AuthResponse>.Ok(result, "Login successful."));
    }

    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), 200)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await Mediator.Send(new RefreshTokenCommand(request.RefreshToken, ip), cancellationToken);
        return Ok(ApiResponse<AuthResponse>.Ok(result, "Token refreshed."));
    }
}
