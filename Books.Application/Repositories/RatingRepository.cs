using Books.Application.Database;
using Books.Application.Models;
using Dapper;

namespace Books.Application.Repositories
{
	public class RatingRepository(IDbConnectionFactory dbConnectionFactory) : IRatingRepository
	{
		public async Task<bool> DeleteRatingAsync(Guid bookId, Guid userId, CancellationToken token = default)
		{
			using var connection = await dbConnectionFactory.CreateConnectionAsync(token);

			var deleteRatingQuery = @"
				DELETE FROM Ratings
				WHERE BookId = @BookId AND UserId = @UserId";

			var command = new CommandDefinition(deleteRatingQuery, new { BookId = bookId, UserId = userId }, cancellationToken: token);
			var result = await connection.ExecuteAsync(command);

			return result > 0;
		}

		public async Task<float?> GetRatingAsync(Guid bookId, CancellationToken token = default)
		{
			using var connection = await dbConnectionFactory.CreateConnectionAsync(token);

			var getRatingQuery = @"
				SELECT ROUND(AVG(CAST(Rating AS FLOAT)), 1)
				FROM Ratings
				WHERE BookId = @BookId";

			var command = new CommandDefinition(getRatingQuery, new { BookId = bookId }, cancellationToken: token);
			return await connection.QuerySingleOrDefaultAsync<float?>(command);
		}

		public async Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid bookId, Guid userId, CancellationToken token = default)
		{
			using var connection = await dbConnectionFactory.CreateConnectionAsync(token);

			var getRatingsQuery = @"
				SELECT ROUND(AVG(CAST(Rating AS FLOAT)), 1), 
				       (SELECT TOP 1 Rating FROM Ratings WHERE BookId = @BookId AND UserId = @UserId)
				FROM Ratings
				WHERE BookId = @BookId";

			var command = new CommandDefinition(getRatingsQuery, new { BookId = bookId, UserId = userId }, cancellationToken: token);
			var result = await connection.QuerySingleOrDefaultAsync<(float?, int?)>(command);

			return result;
		}

		public async Task<IEnumerable<BookRating>> GetRatingsForUserAsync(Guid userId, CancellationToken token = default)
		{
			using var connection = await dbConnectionFactory.CreateConnectionAsync(token);

			var getUserRatingsQuery = @"
				SELECT r.BookId, b.Slug, r.Rating
				FROM Ratings r
				INNER JOIN Books b ON r.BookId = b.Id
				WHERE r.UserId = @UserId";

			var command = new CommandDefinition(getUserRatingsQuery, new { UserId = userId }, cancellationToken: token);
			var result = await connection.QueryAsync<BookRating>(command);

			return result;
		}

		public async Task<bool> RateBookAsync(Guid bookId, int rating, Guid userId, CancellationToken token = default)
		{
			using var connection = await dbConnectionFactory.CreateConnectionAsync(token);

			var ratingQuery = @"
				MERGE INTO Ratings AS target
				USING (SELECT @UserId AS UserId, @BookId AS BookId) AS source
				ON target.UserId = source.UserId AND target.BookId = source.BookId
				WHEN MATCHED THEN 
					UPDATE SET Rating = @Rating
				WHEN NOT MATCHED THEN
					INSERT (UserId, BookId, Rating) VALUES (@UserId, @BookId, @Rating);";

			var command = new CommandDefinition(ratingQuery, new { UserId = userId, BookId = bookId, Rating = rating }, cancellationToken: token);
			var result = await connection.ExecuteAsync(command);

			return result > 0;
		}
	}
}