using FluentValidation;
namespace WLR.Application.Features.Centers.Commands.CreateCenter;
public class CreateCenterCommandValidator : AbstractValidator<CreateCenterCommand>
{
    public CreateCenterCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ContactEmail).EmailAddress().When(x => !string.IsNullOrEmpty(x.ContactEmail));
    }
}
