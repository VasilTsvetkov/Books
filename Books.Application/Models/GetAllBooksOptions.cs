namespace Books.Application.Models
{
	public class GetAllBooksOptions
	{
        public string? Title { get; set; }

        public int? YearOfRelease { get; set; }

        public Guid? UserId { get; set; }
    }
}
