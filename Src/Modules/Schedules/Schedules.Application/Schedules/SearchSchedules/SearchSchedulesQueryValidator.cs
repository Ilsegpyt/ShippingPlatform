using FluentValidation;

namespace Schedules.Application.Schedules.SearchSchedules;

public sealed class SearchSchedulesQueryValidator
    : AbstractValidator<SearchSchedulesQuery>
{
    public SearchSchedulesQueryValidator()
    {
        RuleFor(x => x.Origin)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Destination)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.DepartureDate)
            .NotEmpty();

        RuleFor(x => x.ContainerSize)
            .IsInEnum();
    }
}
