using FluentValidation;

namespace Operations.Application.Operations.UpdatePort;

public sealed class UpdatePortValidator : AbstractValidator<UpdatePortCommand>
{
    public UpdatePortValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().WithMessage("Port name is required.").MaximumLength(160);
        RuleFor(x => x.Code).MaximumLength(20);
    }
}
