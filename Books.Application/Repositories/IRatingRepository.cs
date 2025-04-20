using Books.Application.Models;

namespace Books.Application.Repositories
{
	public interface IRatingRepository
	{
		Task<bool> RateBookAsync(Guid bookId, int rating, Guid userId, CancellationToken token = default);

		Task<float?> GetRatingAsync(Guid bookId, CancellationToken token = default);

		Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid bookId, Guid userId, CancellationToken token = default);

		Task<bool> DeleteRatingAsync(Guid bookId, Guid userId, CancellationToken token = default);

		Task<IEnumerable<BookRating>> GetRatingsForUserAsync(Guid userId, CancellationToken token = default);
	}
}
