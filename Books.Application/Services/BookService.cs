using Books.Application.Models;
using Books.Application.Repositories;
using FluentValidation;

namespace Books.Application.Services
{
	public class BookService(IBookRepository bookRepository, 
		IValidator<Book> bookValidator, 
		IRatingRepository ratingRepository) : IBookService
	{
		public async Task<bool> CreateAsync(Book book, CancellationToken token = default)
		{
			await bookValidator.ValidateAndThrowAsync(book, token);

			return await bookRepository.CreateAsync(book, token);
		}

		public Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
			=> bookRepository.DeleteByIdAsync(id, token);

		public Task<IEnumerable<Book>> GetAllAsync(Guid? userId = default, CancellationToken token = default)
			=> bookRepository.GetAllAsync(userId, token);

		public Task<Book?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken token = default)
			=> bookRepository.GetByIdAsync(id, userId, token);

		public Task<Book?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default)
			=> bookRepository.GetBySlugAsync(slug, userId, token);

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