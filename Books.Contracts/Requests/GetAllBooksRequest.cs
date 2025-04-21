namespace Books.Contracts.Requests
{
	public class GetAllBooksRequest
	{
        public required string? Title { get; init; }

        public required int? Year { get; init; }

        public required string? SortBy { get; init; }
    }
}
