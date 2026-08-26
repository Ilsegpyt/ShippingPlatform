using FluentValidation;

namespace Identity.Application.InternalUsers.UpdateInternalUserProfile;

public sealed class UpdateInternalUserProfileValidator
    : AbstractValidator<UpdateInternalUserProfileCommand>
{
    public UpdateInternalUserProfileValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}