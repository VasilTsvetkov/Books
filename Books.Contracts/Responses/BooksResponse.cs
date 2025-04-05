namespace Books.Contracts.Responses
{
	public class BooksResponse
	{
		public required IEnumerable<BookResponse> Books { get; init; } = Enumerable.Empty<BookResponse>();
    }
}
