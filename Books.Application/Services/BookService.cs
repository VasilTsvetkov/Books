namespace Books.Application.Services
{
	using FluentValidation;
	using Models;
	using Repositories;

	public class BookService(IBookRepository bookRepository,
		IValidator<Book> bookValidator,
		IRatingRepository ratingRepository,
		IValidator<GetAllBooksOptions> optionsValidator) : IBookService
	{
		public async Task<bool> CreateAsync(Book book, CancellationToken token = default)
		{
			await bookValidator.ValidateAndThrowAsync(book, token);

			return await bookRepository.CreateAsync(book, token);
		}

		public Task<bool> DeleteByIdAsync(Guid bookId, CancellationToken token = default)
			=> bookRepository.DeleteByIdAsync(bookId, token);

		public async Task<IEnumerable<Book>> GetAllAsync(GetAllBooksOptions options, CancellationToken token = default)
		{
			await optionsValidator.ValidateAndThrowAsync(options, token);

			return await bookRepository.GetAllAsync(options, token);
		}

		public Task<Book?> GetByIdAsync(Guid bookId, Guid? userId = default, CancellationToken token = default)
			=> bookRepository.GetByIdAsync(bookId, userId, token);

		public Task<Book?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default)
			=> bookRepository.GetBySlugAsync(slug, userId, token);

		public Task<int> GetCountAsync(string? title, int? yearOfRelease, CancellationToken token = default)
			=> bookRepository.GetCountAsync(title, yearOfRelease, token);

		public async Task<Book?> UpdateAsync(Book book, Guid? userId = default, CancellationToken token = default)
		{
			await bookValidator.ValidateAndThrowAsync(book, token);

			var bookExists = await bookRepository.ExistsByIdAsync(book.Id, token);

			if (!bookExists)
			{
				return null;
			}

			await bookRepository.UpdateAsync(book, token);

			if (!userId.HasValue)
			{
				var rating = await ratingRepository.GetRatingAsync(book.Id, token);
				book.Rating = rating;
				return book;
			}

			var ratings = await ratingRepository.GetRatingAsync(book.Id, userId.Value, token);
			book.Rating = ratings.Rating;
			book.UserRating = ratings.UserRating;
			return book;
		}
	}
}