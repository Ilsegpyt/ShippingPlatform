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

        When(x => x.Type == Domain.Entities.ContentType.Category, () =>
        {
            RuleFor(x => x.Body)
                .Empty()
                .WithMessage("Category content cannot have a body.");

            RuleFor(x => x.FeaturedImage)
                .Empty()
                .WithMessage("Category content cannot have a featured image.");

            RuleFor(x => x.LinkUrl)
                .Empty()
                .WithMessage("Category content cannot have a link URL.");
        });

        When(x => x.Type == Domain.Entities.ContentType.Page, () =>
        {
            RuleFor(x => x.Body)
                .NotEmpty()
                .WithMessage("Body is required for Page content.");

            RuleFor(x => x.FeaturedImage)
                .Empty()
                .WithMessage("Page content cannot have a featured image.");

            RuleFor(x => x.LinkUrl)
                .Empty()
                .WithMessage("Page content cannot have a link URL.");
        });

        When(x => x.Type == Domain.Entities.ContentType.Post, () =>
        {
            RuleFor(x => x.Body)
                .NotEmpty()
                .WithMessage("Body is required for Post content.");

            RuleFor(x => x.FeaturedImage)
                .NotEmpty()
                .WithMessage("Featured image is required for Post content.");

            RuleFor(x => x.LinkUrl)
                .Empty()
                .WithMessage("Post content cannot have a link URL.");
        });

        When(x => x.Type == Domain.Entities.ContentType.Link, () =>
        {
            RuleFor(x => x.Body)
                .Empty()
                .WithMessage("Link content cannot have a body.");

            RuleFor(x => x.FeaturedImage)
                .Empty()
                .WithMessage("Link content cannot have a featured image.");

            RuleFor(x => x.LinkUrl)
                .NotEmpty()
                .WithMessage("Link URL is required for Link content.");
        });
    }
}