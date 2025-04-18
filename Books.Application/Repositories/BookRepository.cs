using Books.Application.Database;
using Books.Application.Models;
using Dapper;

namespace Books.Application.Repositories
{
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

		public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
		{
			using var connection = await dbConnectionFactory.CreateConnectionAsync(token);
			using var transaction = connection.BeginTransaction();

			var deleteGenreQuery = @"
				DELETE FROM Genres
				WHERE BookId = @BookId;";

			var genreCommand = new CommandDefinition(deleteGenreQuery, new { BookId = id }, transaction, cancellationToken: token);
			await connection.ExecuteAsync(genreCommand);

			var deleteBookQuery = @"
				DELETE FROM Books
				WHERE Id = @Id;";

			var bookCommand = new CommandDefinition(deleteBookQuery, new { Id = id }, transaction, cancellationToken: token);
			var bookResult = await connection.ExecuteAsync(bookCommand);

			transaction.Commit();
			return bookResult > 0;
		}

		public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default)
		{
			using var connection = await dbConnectionFactory.CreateConnectionAsync(token);

			var checkIfExistsQuery = @"
				SELECT COUNT(1) FROM Books
				WHERE Id = @Id;";

			var command = new CommandDefinition(checkIfExistsQuery, new { Id = id }, cancellationToken: token);
			var result = await connection.ExecuteScalarAsync<int>(command);

			return result > 0;
		}

		public async Task<IEnumerable<Book>> GetAllAsync(CancellationToken token = default)
		{
			using var connection = await dbConnectionFactory.CreateConnectionAsync(token);

			var selectBooksWithGenresQuery = @"
				SELECT b.*, g.Name AS Genre FROM Books b
				LEFT JOIN Genres g ON b.Id = g.BookId;";

			var command = new CommandDefinition(selectBooksWithGenresQuery, cancellationToken: token);
			var result = await connection.QueryAsync<Book>(command);

			return result.Select(x => new Book
			{
				Id = x.Id,
				Title = x.Title,
				Author = x.Author,
				YearOfRelease = x.YearOfRelease,
				Genre = x.Genre
			});
		}

		public async Task<Book?> GetByIdAsync(Guid id, CancellationToken token = default)
		{
			using var connection = await dbConnectionFactory.CreateConnectionAsync(token);

			var selectBookQuery = @"
				SELECT * FROM Books
				WHERE Id = @Id;";

			var bookCommand = new CommandDefinition(selectBookQuery, new { Id = id }, cancellationToken: token);
			var book = await connection.QuerySingleOrDefaultAsync<Book>(bookCommand);

			if (book is null)
			{
				return null;
			}

			var selectGenresQuery = @"
				SELECT Name FROM Genres
				WHERE BookId = @BookId;";

			var genreCommand = new CommandDefinition(selectGenresQuery, new { BookId = id }, cancellationToken: token);
			var genre = await connection.QueryFirstAsync<string>(genreCommand);

			book.Genre = genre;
			return book;
		}

		public async Task<Book?> GetBySlugAsync(string slug, CancellationToken token = default)
		{
			using var connection = await dbConnectionFactory.CreateConnectionAsync(token);

			var selectBookQuery = @"
				SELECT * FROM Books
				WHERE Slug = @Slug;";

			var bookCommand = new CommandDefinition(selectBookQuery, new { Slug = slug }, cancellationToken: token);
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