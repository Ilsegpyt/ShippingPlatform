using FluentValidation;

namespace Content.Application.Content.UpdateContent;

public sealed class UpdateContentCommandValidator
    : AbstractValidator<UpdateContentCommand>
{
    public UpdateContentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

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