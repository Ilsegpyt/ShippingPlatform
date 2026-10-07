using FluentValidation;

namespace Website.Application.QuoteRequests.CreateQuoteRequest;

public sealed class CreateQuoteRequestCommandValidator
    : AbstractValidator<CreateQuoteRequestCommand>
{
    public CreateQuoteRequestCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Company)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Country)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.CountryCode)
            .NotEmpty()
            .MaximumLength(10);

        RuleFor(x => x.Phone)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(200);

        RuleFor(x => x.Message)
            .NotEmpty()
            .MaximumLength(2000);

        RuleFor(x => x.InterestType)
            .IsInEnum();

        RuleFor(x => x.TransportModes)
            .NotEmpty()
            .Must(modes => modes.All(mode => Enum.IsDefined(mode)))
            .WithMessage("At least one valid transport mode is required.");

        RuleFor(x => x.AnnualShipments)
            .IsInEnum();
    }
}