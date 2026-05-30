using FluentValidation;
namespace WLR.Application.Features.Motors.Commands.CreateMotor;
public class CreateMotorCommandValidator : AbstractValidator<CreateMotorCommand>
{
    public CreateMotorCommandValidator()
    {
        RuleFor(x => x.LocationId).NotEmpty();
        RuleFor(x => x.MotorNumber).NotEmpty().MaximumLength(100);
        RuleFor(x => x.WaterCapacityLiters).GreaterThan(0);
    }
}
