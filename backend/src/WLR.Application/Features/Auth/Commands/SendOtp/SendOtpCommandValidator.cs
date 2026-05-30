using FluentValidation;
namespace WLR.Application.Features.Auth.Commands.SendOtp;

public class SendOtpCommandValidator : AbstractValidator<SendOtpCommand>
{
    public SendOtpCommandValidator()
    {
        RuleFor(x => x.MobileNumber)
            .NotEmpty().WithMessage("Mobile number is required.")
            .Matches(@"^\+?[1-9]\d{9,14}$").WithMessage("Invalid mobile number format.");
    }
}
