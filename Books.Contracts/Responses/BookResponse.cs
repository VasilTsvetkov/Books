namespace Books.Contracts.Responses
{
	public class BookResponse
	{
		public required Guid Id { get; init; }

		public required string Title { get; init; }

        public required string Slug { get; init; }

		public required string Author { get; init; }

		public required int YearOfRelease { get; init; }

        public required string Genre { get; init; }
	}
}
