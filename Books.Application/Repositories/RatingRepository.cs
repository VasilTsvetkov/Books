using Books.Application.Database;
using Dapper;

namespace Books.Application.Repositories
{
	public class RatingRepository(IDbConnectionFactory dbConnectionFactory) : IRatingRepository
	{
		public async Task<float?> GetRatingAsync(Guid bookId, CancellationToken token = default)
		{
			using var connection = await dbConnectionFactory.CreateConnectionAsync(token);

			var query = @"
				SELECT ROUND(AVG(CAST(Rating AS FLOAT)), 1)
				FROM Ratings
				WHERE BookId = @BookId";

			var command = new CommandDefinition(query, new { BookId = bookId }, cancellationToken: token);
			return await connection.QuerySingleOrDefaultAsync<float?>(command);
		}

		public async Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid bookId, Guid userId, CancellationToken token = default)
		{
			using var connection = await dbConnectionFactory.CreateConnectionAsync(token);

			var query = @"
				SELECT ROUND(AVG(CAST(Rating AS FLOAT)), 1), 
				       (SELECT Rating FROM Ratings WHERE BookId = @BookId AND UserId = @UserId LIMIT 1)
				FROM Ratings
				WHERE BookId = @BookId";

			var command = new CommandDefinition(query, new { BookId = bookId, UserId = userId }, cancellationToken: token);
			var result = await connection.QuerySingleOrDefaultAsync<(float?, int?)>(command);

			return result;
		}
	}
}