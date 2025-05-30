﻿namespace Books.Application.Services
{
	using FluentValidation;
	using FluentValidation.Results;
	using Models;
	using Repositories;

	public class RatingService(IRatingRepository ratingRepository, IBookRepository bookRepository) : IRatingService
	{
		public async Task<bool> DeleteRatingAsync(Guid bookId, Guid userId, CancellationToken token = default)
			=> await ratingRepository.DeleteRatingAsync(bookId, userId, token);

		public Task<IEnumerable<BookRating>> GetRatingsForUserAsync(Guid userId, CancellationToken token = default)
			=> ratingRepository.GetRatingsForUserAsync(userId, token);

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