using Books.Application.Models;
using Books.Contracts.Requests;
using Books.Contracts.Responses;

namespace Books.Api.Mapping
{
	public static class ContractMapping
	{
		public static Book MapToBook(this UpsertBookRequest request)
		{
			return new Book
			{
				Id = Guid.NewGuid(),
				Title = request.Title,
				Author = request.Author,
				YearOfRelease = request.YearOfRelease,
				Genre = request.Genre
			};
		}

		public static Book MapToBook(this UpsertBookRequest request, Guid id)
		{
			return new Book
			{
				Id = id,
				Title = request.Title,
				Author = request.Author,
				YearOfRelease = request.YearOfRelease,
				Genre = request.Genre
			};
		}

		public static BookResponse MapToResponse(this Book book)
		{
			return new BookResponse
			{
				Id = book.Id,
				Title = book.Title,
				Author = book.Author,
				YearOfRelease = book.YearOfRelease,
				Genre = book.Genre
			};
		}

		public static BooksResponse MapToResponse(this IEnumerable<Book> books)
		{
			return new BooksResponse
			{
				Books = books.Select(MapToResponse)
			};
		}
	}
}
