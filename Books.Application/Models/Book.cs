﻿using System.Text.RegularExpressions;

namespace Books.Application.Models
{
	public partial class Book
	{
		public required Guid Id { get; init; }

		public required string Title { get; set; }

		public string Slug => GenerateSlug();

		public required string Author { get; set; }

		public required int YearOfRelease { get; set; }

		public required string Genre { get; set; }

		private string GenerateSlug()
		{
			var sluggedTitle = SlugRegex().Replace(Title, string.Empty)
				.ToLower().Replace(" ", "-");
			return $"{sluggedTitle}-{YearOfRelease}";
		}

		[GeneratedRegex("[^0-9A-Za-z _-]", RegexOptions.NonBacktracking, 5)]
		private static partial Regex SlugRegex();
	}
}
