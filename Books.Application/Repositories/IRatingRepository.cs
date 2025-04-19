namespace Books.Application.Repositories
{
	public interface IRatingRepository
	{
		Task<float?> GetRatingAsync(Guid bookId, CancellationToken token = default);

		Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid bookId, Guid userId, CancellationToken token = default);
	}
}
