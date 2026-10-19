using FluentValidation;

namespace Operations.Application.Operations.ApproveFclSuggestion;

public sealed class ApproveFclSuggestionValidator : AbstractValidator<ApproveFclSuggestionCommand>
{
    public ApproveFclSuggestionValidator()
    {
        RuleFor(x => x.SuggestionId).NotEmpty().WithMessage("FCL suggestion is required.");
    }
}
