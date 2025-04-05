namespace Books.Application.Models
{
	public class Book
	{
		public required Guid Id { get; init; }

		public required string Title { get; set; }

		public required string Author { get; set; }

		public required int YearOfRelease { get; set; }

		public required string Genre { get; set; }
	}
}
