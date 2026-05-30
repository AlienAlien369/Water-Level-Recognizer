using MediatR;
namespace WLR.Application.Features.Auth.Commands.SendOtp;
public record SendOtpCommand(string MobileNumber) : IRequest<bool>;
