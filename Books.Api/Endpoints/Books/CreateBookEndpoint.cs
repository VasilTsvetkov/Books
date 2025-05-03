namespace Books.Api.Endpoints.Books
{
	using Application.Services;
	using Auth;
	using Contracts.Requests;
	using Contracts.Responses;
	using Microsoft.AspNetCore.OutputCaching;

	public static class CreateBookEndpoint
	{
		public const string Name = "CreateBook";

		public static IEndpointRouteBuilder MapCreateBook(this IEndpointRouteBuilder app)
		{
			app.MapPost(ApiEndpoints.Books.Create, async (
					UpsertBookRequest request, IBookService bookService,
					IOutputCacheStore outputCacheStore, CancellationToken token) =>
			{
				var book = request.MapToBook();
				await bookService.CreateAsync(book, token);
				await outputCacheStore.EvictByTagAsync("books", token);

				return TypedResults.CreatedAtRoute(book.MapToResponse(), GetBookEndpoint.Name, new { idOrSlug = book.Id });
			})
				.WithName(Name)
				.Produces<BookResponse>(StatusCodes.Status201Created)
				.Produces<ValidationFailureResponse>(StatusCodes.Status400BadRequest)
				.RequireAuthorization(AuthConstants.TrustedMemberPolicyName);

			return app;
		}
	}
}