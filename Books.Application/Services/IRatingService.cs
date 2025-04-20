namespace Books.Application.Services
{
	public interface IRatingService
	{
		Task<bool> RateBookAsync(Guid bookId, int rating, Guid userId, CancellationToken token = default);
	}
}
