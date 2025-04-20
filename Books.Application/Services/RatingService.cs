using Books.Application.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace Books.Application.Services
{
	public class RatingService(IRatingRepository ratingRepository, IBookRepository bookRepository) : IRatingService
	{
		public async Task<bool> RateBookAsync(Guid bookId, int rating, Guid userId, CancellationToken token = default)
		{
			if (rating is <= 0 or > 5)
			{
				throw new ValidationException(new[]
				{
					new ValidationFailure
					{
						PropertyName = "Rating",
						ErrorMessage = "Rating must be between 1 and 5"
					}
				});
			}

			var bookExists = await bookRepository.ExistsByIdAsync(bookId, token);
			if (!bookExists)
			{
				return false;
			}

			return await ratingRepository.RateBookAsync(bookId, rating, userId, token);
		}
	}
}