using Books.Application.Models;
using FluentValidation;

namespace Books.Application.Validators
{
	public class GetAllBooksOptionsValidator : AbstractValidator<GetAllBooksOptions>
	{
        public GetAllBooksOptionsValidator()
        {
			RuleFor(x => x.YearOfRelease)
				.LessThanOrEqualTo(DateTime.UtcNow.Year);
		}
    }
}
