﻿namespace Books.Contracts.Responses
{
	public class BookRatingResponse
	{
		public required Guid BookId { get; init; }
			   
		public required string Slug { get; init; }
			   
		public required int Rating { get; init; }
	}
}
