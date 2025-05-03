namespace Books.Api.Endpoints.Books
{
	using Application.Services;
	using Auth;
	using Contracts.Requests;
	using Contracts.Responses;
	using Mapping;
	using Microsoft.AspNetCore.OutputCaching;

	public static class UpdateBookEndpoint
	{
		public const string Name = "UpdateBook";

		public static IEndpointRouteBuilder MapUpdateBook(this IEndpointRouteBuilder app)
		{
			app.MapPut(ApiEndpoints.Books.Update, async (
					Guid id, UpsertBookRequest request, IBookService bookService,
					IOutputCacheStore outputCacheStore, HttpContext context, CancellationToken token) =>
			{
				var book = request.MapToBook(id);
				var userId = context.GetUserId();
				var updatedBook = await bookService.UpdateAsync(book, userId, token);

				if (updatedBook is null)
				{
					return Results.NotFound();
				}

				await outputCacheStore.EvictByTagAsync("books", token);

				return TypedResults.Ok(updatedBook.MapToResponse());
			})
				.WithName(Name)
				.Produces<BookResponse>(StatusCodes.Status200OK)
				.Produces(StatusCodes.Status404NotFound)
				.Produces<ValidationFailureResponse>(StatusCodes.Status400BadRequest)
				.RequireAuthorization(AuthConstants.TrustedMemberPolicyName);

			return app;
		}
	}
}
