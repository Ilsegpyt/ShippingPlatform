
using FluentValidation;

namespace Operations.Application.Operations.GenerateFclSuggestions;

public sealed class GenerateFclSuggestionsValidator
    : AbstractValidator<GenerateFclSuggestionsCommand>
{
    public GenerateFclSuggestionsValidator()
    {
        RuleFor(x => x.ExportContainerId)
            .NotEmpty();
    }
}
