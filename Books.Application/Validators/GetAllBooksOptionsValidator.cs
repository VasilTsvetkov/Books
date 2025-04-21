using Books.Application.Models;
using FluentValidation;

namespace Books.Application.Validators
{
	public class GetAllBooksOptionsValidator : AbstractValidator<GetAllBooksOptions>
	{
		private static readonly string[] AcceptableSortFields =
		{
			"title", "yearOfRelease"
		};

        public GetAllBooksOptionsValidator()
        {
			RuleFor(x => x.YearOfRelease)
				.LessThanOrEqualTo(DateTime.UtcNow.Year);

			RuleFor(x => x.SortField)
				.Must(x => x is null || AcceptableSortFields.Contains(x, StringComparer.OrdinalIgnoreCase))
				.WithMessage("You can only sort by 'title' or 'yearOfRelease'");
		}
    }
}
