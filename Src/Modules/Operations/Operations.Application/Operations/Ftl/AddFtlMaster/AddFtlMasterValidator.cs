using FluentValidation;

namespace Operations.Application.Operations.Ftl.AddFtlMaster;

public sealed class AddFtlMasterValidator : AbstractValidator<AddFtlMasterCommand>
{
    public AddFtlMasterValidator()
    {
        RuleFor(x => x.ListName)
            .NotEmpty()
            .WithMessage("List name is required.")
            .MaximumLength(50);

        RuleFor(x => x.Value)
            .NotEmpty()
            .WithMessage("Value is required.")
            .MaximumLength(200);
    }
}

