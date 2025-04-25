namespace Books.Contracts.Requests
{
	public class GetAllBooksRequest : PagedRequest
	{
        public string? Title { get; init; }

        public int? Year { get; init; }

        public string? SortBy { get; init; }
    }
}
