namespace Books.Application.Repositories
{
	using Dapper;
	using Database;
	using Models;

	public class BookRepository(IDbConnectionFactory dbConnectionFactory) : IBookRepository
	{
		public async Task<bool> CreateAsync(Book book, CancellationToken token = default)
		{
			using var connection = await dbConnectionFactory.CreateConnectionAsync(token);
			using var transaction = connection.BeginTransaction();

			var insertBookQuery = @"
				INSERT INTO Books (Id, Title, Slug, Author, YearOfRelease)
				VALUES (@Id, @Title, @Slug, @Author, @YearOfRelease);";

			var bookParams = new
			{
				book.Id,
				book.Title,
				book.Slug,
				book.Author,
				book.YearOfRelease,
			};

			var insertBookCommand = new CommandDefinition(insertBookQuery, bookParams, transaction, cancellationToken: token);
			var result = await connection.ExecuteAsync(insertBookCommand);

			if (result > 0)
			{
				var insertGenreQuery = @"
					INSERT INTO Genres (BookId, Name)
					VALUES (@BookId, @Name);";

				var genreParams = new
				{
					BookId = book.Id,
					Name = book.Genre
				};

				var insertGenreCommand = new CommandDefinition(insertGenreQuery, genreParams, transaction, cancellationToken: token);
				await connection.ExecuteAsync(insertGenreCommand);
			}

			transaction.Commit();
			return result > 0;
		}

		public async Task<bool> DeleteByIdAsync(Guid bookId, CancellationToken token = default)
		{
			using var connection = await dbConnectionFactory.CreateConnectionAsync(token);
			using var transaction = connection.BeginTransaction();

			var deleteGenreQuery = @"
				DELETE FROM Genres
				WHERE BookId = @BookId;";

			var genreCommand = new CommandDefinition(deleteGenreQuery, new { BookId = bookId }, transaction, cancellationToken: token);
			await connection.ExecuteAsync(genreCommand);

			var deleteBookQuery = @"
				DELETE FROM Books
				WHERE Id = @Id;";

			var bookCommand = new CommandDefinition(deleteBookQuery, new { Id = bookId }, transaction, cancellationToken: token);
			var bookResult = await connection.ExecuteAsync(bookCommand);

			transaction.Commit();
			return bookResult > 0;
		}

		public async Task<bool> ExistsByIdAsync(Guid bookId, CancellationToken token = default)
		{
			using var connection = await dbConnectionFactory.CreateConnectionAsync(token);

			var checkIfExistsQuery = @"
				SELECT COUNT(1) FROM Books
				WHERE Id = @Id;";

			var command = new CommandDefinition(checkIfExistsQuery, new { Id = bookId }, cancellationToken: token);
			var result = await connection.ExecuteScalarAsync<int>(command);

			return result > 0;
		}

		public async Task<IEnumerable<Book>> GetAllAsync(GetAllBooksOptions options, CancellationToken token = default)
		{
			using var connection = await dbConnectionFactory.CreateConnectionAsync(token);
			var orderClause = "ORDER BY b.Title";

			if (options.SortField is not null)
			{
				orderClause = $"""
					ORDER BY b.{options.SortField} {(options.SortOrder == SortOrder.Ascending ? "ASC" : "DESC")}
					""";
			}

			var query = $"""
				SELECT 
					b.*,
					MAX(g.Name) AS Genre,
					ROUND(AVG(CAST(r.Rating AS FLOAT)), 1) AS Rating,
					myr.Rating AS UserRating
				FROM Books b
				LEFT JOIN Genres g ON b.Id = g.BookId
				LEFT JOIN Ratings r ON b.Id = r.BookId
				LEFT JOIN Ratings myr ON b.Id = myr.BookId AND myr.UserId = @UserId
				WHERE (@Title IS NULL OR LOWER(b.Title) LIKE LOWER('%' + @Title + '%'))
				  AND (@YearOfRelease IS NULL OR b.YearOfRelease = @YearOfRelease)
				GROUP BY b.Id, b.Title, b.Slug, b.Author, b.YearOfRelease, myr.Rating 
				{orderClause}
				OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
				""";

			var parameters = new
			{
				UserId = options.UserId,
				Title = options.Title,
				YearOfRelease = options.YearOfRelease,
				PageSize = options.PageSize,
				Offset = (options.Page - 1) * options.PageSize
			};

			var command = new CommandDefinition(query, parameters, cancellationToken: token);
			var result = await connection.QueryAsync<Book>(command);
			return result;
		}

		public async Task<Book?> GetByIdAsync(Guid bookId, Guid? userId = default, CancellationToken token = default)
		{
			using var connection = await dbConnectionFactory.CreateConnectionAsync(token);

			var selectBookQuery = @"
				SELECT 
					b.*, 
					ROUND(AVG(r.Rating * 1.0), 1) AS Rating, 
					myr.Rating AS UserRating
				FROM Books b
				LEFT JOIN Ratings r ON b.Id = r.BookId
				LEFT JOIN Ratings myr ON b.Id = myr.BookId AND myr.UserId = @UserId
				WHERE b.Id = @Id
				GROUP BY b.Id, b.Title, b.Slug, b.Author, b.YearOfRelease, myr.Rating;";

			var bookCommand = new CommandDefinition(selectBookQuery, new { Id = bookId, UserId = userId }, cancellationToken: token);
			var book = await connection.QuerySingleOrDefaultAsync<Book>(bookCommand);

			if (book is null)
			{
				return null;
			}

			var selectGenresQuery = @"
				SELECT Name FROM Genres
				WHERE BookId = @BookId;";

			var genreCommand = new CommandDefinition(selectGenresQuery, new { BookId = bookId }, cancellationToken: token);
			var genre = await connection.QueryFirstAsync<string>(genreCommand);

			book.Genre = genre;
			return book;
		}

		public async Task<Book?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default)
		{
			using var connection = await dbConnectionFactory.CreateConnectionAsync(token);

			var selectBookQuery = @"
				SELECT 
					b.*, 
					ROUND(AVG(r.Rating * 1.0), 1) AS Rating, 
					myr.Rating AS UserRating
				FROM Books b
				LEFT JOIN Ratings r ON b.Id = r.BookId
				LEFT JOIN Ratings myr ON b.Id = myr.BookId AND myr.UserId = @UserId
				WHERE b.Slug = @Slug
				GROUP BY b.Id, b.Title, b.Slug, b.Author, b.YearOfRelease, myr.Rating;";

			var bookCommand = new CommandDefinition(selectBookQuery, new { Slug = slug, UserId = userId }, cancellationToken: token);
			var book = await connection.QuerySingleOrDefaultAsync<Book>(bookCommand);

			if (book is null)
			{
				return null;
			}

			var selectGenresQuery = @"
				SELECT Name FROM Genres
				WHERE BookId = @BookId;";

			var genreCommand = new CommandDefinition(selectGenresQuery, new { BookId = book.Id }, cancellationToken: token);
			var genre = await connection.QueryFirstAsync<string>(genreCommand);

			book.Genre = genre;
			return book;
		}

		public async Task<int> GetCountAsync(string? title, int? yearOfRelease, CancellationToken token = default)
		{
			using var connection = await dbConnectionFactory.CreateConnectionAsync(token);

			var query = $"""
				SELECT COUNT(*)
				FROM Books b
				WHERE (@Title IS NULL OR b.Title LIKE '%' + @Title + '%')
				  AND (@YearOfRelease IS NULL OR b.YearOfRelease = @YearOfRelease)
				""";

			var parameters = new
			{
				Title = title,
				YearOfRelease = yearOfRelease
			};

			var command = new CommandDefinition(query, parameters, cancellationToken: token);
			var count = await connection.ExecuteScalarAsync<int>(command);
			return count;
		}

		public async Task<bool> UpdateAsync(Book book, CancellationToken token = default)
		{
			using var connection = await dbConnectionFactory.CreateConnectionAsync(token);
			using var transaction = connection.BeginTransaction();

			var deleteGenreQuery = @"
				DELETE FROM Genres
				WHERE BookId = @BookId;";

			var deleteGenreCommand = new CommandDefinition(deleteGenreQuery, new { BookId = book.Id }, transaction, cancellationToken: token);
			await connection.ExecuteAsync(deleteGenreCommand);

			var insertGenreQuery = @"
				INSERT INTO Genres (BookId, Name)
				VALUES (@BookId, @Name);";

			var genreParams = new
			{
				BookId = book.Id,
				Name = book.Genre
			};

			var insertGenreCommand = new CommandDefinition(insertGenreQuery, genreParams, transaction, cancellationToken: token);
			await connection.ExecuteAsync(insertGenreCommand);

			var updateBookQuery = @"
				UPDATE Books
				SET Title = @Title, Slug = @Slug, Author = @Author, YearOfRelease = @YearOfRelease
				WHERE Id = @Id;";

			var bookParams = new
			{
				book.Id,
				book.Title,
				book.Slug,
				book.Author,
				book.YearOfRelease
			};

			var updateBookCommand = new CommandDefinition(updateBookQuery, bookParams, transaction, cancellationToken: token);
			var result = await connection.ExecuteAsync(updateBookCommand);

			transaction.Commit();
			return result > 0;
		}
	}
}