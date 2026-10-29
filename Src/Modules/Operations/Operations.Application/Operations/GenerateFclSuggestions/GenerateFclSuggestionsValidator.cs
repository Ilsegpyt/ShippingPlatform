using FluentValidation;

namespace Operations.Application.Operations.GenerateFclSuggestions;

public sealed class GenerateFclSuggestionsValidator
    : AbstractValidator<GenerateFclSuggestionsCommand>
{
    public GenerateFclSuggestionsValidator()
    {
        When(x => x.ExportContainerId.HasValue, () =>
        {
            RuleFor(x => x.ExportContainerId)
                .NotEmpty();
        });
    }
}
