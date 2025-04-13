using Books.Application.Database;
using Books.Application.Models;
using Dapper;

namespace Books.Application.Repositories
{
	public class BookRepository(IDbConnectionFactory dbConnectionFactory) : IBookRepository
	{
		public async Task<bool> CreateAsync(Book book)
		{
			using var connection = await dbConnectionFactory.CreateConnectionAsync();
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

			var result = await connection.ExecuteAsync(insertBookQuery, bookParams, transaction);

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

				await connection.ExecuteAsync(insertGenreQuery, genreParams, transaction);
			}

			transaction.Commit();
			return result > 0;
		}

		public async Task<bool> DeleteByIdAsync(Guid id)
		{
			using var connection = await dbConnectionFactory.CreateConnectionAsync();
			using var transaction = connection.BeginTransaction();

			var deleteGenreQuery = @"
				DELETE FROM Genres
				WHERE BookId = @BookId;";

			var genreResult = await connection.ExecuteAsync(deleteGenreQuery, new { BookId = id }, transaction);

			var deleteBookQuery = @"
				DELETE FROM Books
				WHERE Id = @Id;";

			var bookResult = await connection.ExecuteAsync(deleteBookQuery, new { Id = id }, transaction);

			transaction.Commit();

			return bookResult > 0;
		}

		public async Task<bool> ExistsByIdAsync(Guid id)
		{
			using var connection = await dbConnectionFactory.CreateConnectionAsync();

			var checkIfExistsQuery = @"
				SELECT COUNT(1) FROM Books
				WHERE Id = @Id;";

			var result = await connection.ExecuteScalarAsync<int>(checkIfExistsQuery, new { Id = id });

			return result > 0;
		}

		public async Task<IEnumerable<Book>> GetAllAsync()
		{
			using var connection = await dbConnectionFactory.CreateConnectionAsync();

			var selectBooksWithGenresQuery = @"
				SELECT b.*, g.Name AS Genre FROM Books b
				LEFT JOIN Genres g ON b.Id = g.BookId;";

			var result = await connection.QueryAsync<Book>(selectBooksWithGenresQuery);

			return result.Select(x => new Book
			{
				Id = x.Id,
				Title = x.Title,
				Author = x.Author,
				YearOfRelease = x.YearOfRelease,
				Genre = x.Genre
			});
		}

		public async Task<Book?> GetByIdAsync(Guid id)
		{
			using var connection = await dbConnectionFactory.CreateConnectionAsync();

			var selectBookQuery = @"
				SELECT * FROM Books
				WHERE Id = @Id;";

			var book = await connection.QuerySingleOrDefaultAsync<Book>(selectBookQuery, new { Id = id });

			if (book is null)
			{
				return null;
			}

			var selectGenresQuery = @"
				SELECT Name FROM Genres
				WHERE BookId = @BookId;";

			var genre = await connection.QueryFirstAsync<string>(selectGenresQuery, new { BookId = id });

			book.Genre = genre;

			return book;
		}

		public async Task<Book?> GetBySlugAsync(string slug)
		{
			using var connection = await dbConnectionFactory.CreateConnectionAsync();

			var selectBookQuery = @"
				SELECT * FROM Books
				WHERE Slug = @Slug;";

			var book = await connection.QuerySingleOrDefaultAsync<Book>(selectBookQuery, new { Slug = slug });

			if (book is null)
			{
				return null;
			}

			var selectGenresQuery = @"
				SELECT Name FROM Genres
				WHERE BookId = @BookId;";

			var genre = await connection.QueryFirstAsync<string>(selectGenresQuery, new { BookId = book.Id });

			book.Genre = genre;

			return book;
		}

		public async Task<bool> UpdateAsync(Book book)
		{
			using var connection = await dbConnectionFactory.CreateConnectionAsync();
			using var transaction = connection.BeginTransaction();

			var deleteGenreQuery = @"
				DELETE FROM Genres
				WHERE BookId = @BookId;";

			await connection.ExecuteAsync(deleteGenreQuery, new { BookId = book.Id }, transaction);

			var insertGenreQuery = @"
				INSERT INTO Genres (BookId, Name)
				VALUES (@BookId, @Name);";

			var genreParams = new
			{
				BookId = book.Id,
				Name = book.Genre
			};

			await connection.ExecuteAsync(insertGenreQuery, genreParams, transaction);

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

			var result = await connection.ExecuteAsync(updateBookQuery, bookParams, transaction);

			transaction.Commit();
			return result > 0;
		}
	}
}
