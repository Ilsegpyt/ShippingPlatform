using FluentValidation;

namespace Identity.Application.SubAccounts.UpdateSubAccountProfile;

public sealed class UpdateSubAccountProfileValidator
    : AbstractValidator<UpdateSubAccountProfileCommand>
{
    public UpdateSubAccountProfileValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);
    }
}