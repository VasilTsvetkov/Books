namespace Books.Application.Validators
{
	using FluentValidation;
	using Models;
	using Repositories;

	public class BookValidator : AbstractValidator<Book>
	{
		private readonly IBookRepository _bookRepository;

		public BookValidator(IBookRepository bookRepository)
		{
			_bookRepository = bookRepository;

			RuleFor(x => x.Id)
				.NotEmpty();

			RuleFor(x => x.Title)
				.NotEmpty();

			RuleFor(x => x.Author)
				.NotEmpty();

			RuleFor(x => x.Genre)
				.NotEmpty();

			RuleFor(x => x.YearOfRelease)
				.LessThanOrEqualTo(DateTime.UtcNow.Year);

			RuleFor(x => x.Slug)
				.MustAsync(ValidateSlug)
				.WithMessage("This book already exists");
		}

		private async Task<bool> ValidateSlug(Book book, string slug, CancellationToken token = default)
		{
			var existingBook = await _bookRepository.GetBySlugAsync(slug);
			return existingBook is null || existingBook.Id == book.Id;
		}
	}
}
