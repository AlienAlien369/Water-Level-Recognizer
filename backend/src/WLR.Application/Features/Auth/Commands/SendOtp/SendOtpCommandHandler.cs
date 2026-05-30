using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WLR.Application.Common.Interfaces;
using WLR.Domain.Entities;

namespace WLR.Application.Features.Auth.Commands.SendOtp;

public class SendOtpCommandHandler : IRequestHandler<SendOtpCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IOtpService _otpService;
    private readonly ILogger<SendOtpCommandHandler> _logger;

    public SendOtpCommandHandler(IApplicationDbContext context, IOtpService otpService, ILogger<SendOtpCommandHandler> logger)
    {
        _context = context;
        _otpService = otpService;
        _logger = logger;
    }

    public async Task<bool> Handle(SendOtpCommand request, CancellationToken cancellationToken)
    {
        var existingOtps = await _context.OtpVerifications
            .Where(o => o.MobileNumber == request.MobileNumber && !o.IsUsed && o.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        foreach (var old in existingOtps)
            old.MarkUsed();

        var otpCode = _otpService.GenerateOtp();
        var otp = OtpVerification.Create(request.MobileNumber, otpCode, "Login");
        _context.OtpVerifications.Add(otp);
        await _context.SaveChangesAsync(cancellationToken);

        await _otpService.SendOtpAsync(request.MobileNumber, otpCode, cancellationToken);
        _logger.LogInformation("OTP sent to {MobileNumber}", request.MobileNumber);
        return true;
    }
}
