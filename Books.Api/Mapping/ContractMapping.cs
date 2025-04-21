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
				Slug = book.Slug,
				Rating = book.Rating,
				UserRating = book.UserRating,
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

		public static IEnumerable<BookRatingResponse> MapToResponse(this IEnumerable<BookRating> ratings)
		{
			return ratings.Select(x => new BookRatingResponse
			{
				BookId = x.BookId,
				Slug = x.Slug,
				Rating = x.Rating,
			});
		}

		public static GetAllBooksOptions MapToOptions(this GetAllBooksRequest request)
		{
			return new GetAllBooksOptions
			{
				Title = request.Title,
				YearOfRelease = request.Year,
				SortField = request.SortBy?.Trim('+', '-'),
				SortOrder = request.SortBy is null ? SortOrder.Unsorted :
					request.SortBy.StartsWith('-') ? SortOrder.Descending : SortOrder.Ascending
			};
		}

		public static GetAllBooksOptions WithUser(this GetAllBooksOptions options, Guid? userId)
		{
			options.UserId = userId;
			return options;
		}
	}
}
