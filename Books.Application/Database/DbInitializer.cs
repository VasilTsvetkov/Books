using Dapper;

namespace Books.Application.Database
{
	public class DbInitializer
	{
		private readonly IDbConnectionFactory _connectionFactory;

		public DbInitializer(IDbConnectionFactory connectionFactory)
		{
			_connectionFactory = connectionFactory;
		}

		public async Task InitializeAsync()
		{
			using (var connection = await _connectionFactory.CreateConnectionAsync())
			{
				var createDatabaseQuery = @"
                    IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'BooksDB')
						BEGIN
							CREATE DATABASE BooksDB;
						END";
				await connection.ExecuteAsync(createDatabaseQuery);

				var useDatabaseQuery = "USE BooksDB;";
				await connection.ExecuteAsync(useDatabaseQuery);

				var createBooksTableQuery = @"
					IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Books' AND xtype = 'U')
						BEGIN
							CREATE TABLE Books (
								Id UNIQUEIDENTIFIER PRIMARY KEY, 
								Title NVARCHAR(255) NOT NULL,
								Slug NVARCHAR(255) NOT NULL,
								Author NVARCHAR(255) NOT NULL,
								YearOfRelease INT NOT NULL,
							);
						END";

				await connection.ExecuteAsync(createBooksTableQuery);

				var createIndexQuery = @"
					IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Books_Slug' AND object_id = OBJECT_ID('Books'))
						BEGIN
							CREATE UNIQUE INDEX IX_Books_Slug
							ON Books (Slug);
						END";

				await connection.ExecuteAsync(createIndexQuery);

				var createGenresTableQuery = @"
					IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Genres' AND xtype = 'U')
						BEGIN
							CREATE TABLE Genres (
								BookId UNIQUEIDENTIFIER, 
								Name NVARCHAR(100) NOT NULL,
								CONSTRAINT FK_Genres_Books FOREIGN KEY (BookId) REFERENCES Books(Id)
							);
						END";

				await connection.ExecuteAsync(createGenresTableQuery);
			}
		}
	}
}