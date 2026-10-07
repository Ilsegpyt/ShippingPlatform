using FluentValidation;

namespace Website.Application.Branches.CreateBranch;

public sealed class CreateBranchCommandValidator
    : AbstractValidator<CreateBranchCommand>
{
    public CreateBranchCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Address)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180);

        RuleFor(x => x.CeoName)
            .MaximumLength(200);

        RuleFor(x => x.CeoEmail)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.CeoEmail))
            .MaximumLength(200);

        RuleFor(x => x.GmName)
            .MaximumLength(200);

        RuleFor(x => x.GmEmail)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.GmEmail))
            .MaximumLength(200);

        RuleFor(x => x.BranchManagerName)
            .MaximumLength(200);

        RuleFor(x => x.BranchManagerEmail)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.BranchManagerEmail))
            .MaximumLength(200);
    }
}