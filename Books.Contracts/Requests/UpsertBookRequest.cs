namespace Books.Contracts.Requests
{
	public class UpsertBookRequest
	{
        public required string Title { get; init; }

        public required string Author { get; init; }

        public required int YearOfRelease { get; init; }

        public required string Genre { get; init; }
    }
}
