using FluentValidation;

namespace Website.Application.RecruitmentApplications.CreateRecruitmentApplication;

public sealed class CreateRecruitmentApplicationCommandValidator
    : AbstractValidator<CreateRecruitmentApplicationCommand>
{
    public CreateRecruitmentApplicationCommandValidator()
    {
        RuleFor(x => x.Department)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.CvFileName)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.CvFilePath)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Message)
            .NotEmpty()
            .MaximumLength(2000);
    }
}