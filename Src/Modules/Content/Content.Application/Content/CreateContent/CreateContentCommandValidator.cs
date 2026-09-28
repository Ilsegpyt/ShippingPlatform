using FluentValidation;

namespace Content.Application.Content.CreateContent;

public sealed class CreateContentCommandValidator
    : AbstractValidator<CreateContentCommand>
{
    public CreateContentCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Type)
            .IsInEnum();

        RuleFor(x => x.Body)
            .MaximumLength(100000);

        RuleFor(x => x.FeaturedImage)
            .MaximumLength(1000);

        RuleFor(x => x.LinkUrl)
            .MaximumLength(2000);
    }
}