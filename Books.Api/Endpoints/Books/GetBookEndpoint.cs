using Books.Api.Auth;
using Books.Api.Mapping;
using Books.Application.Services;
using Books.Contracts.Responses;

namespace Books.Api.Endpoints.Books
{
	public static class GetBookEndpoint
	{
		public const string Name = "GetBook";

		public static IEndpointRouteBuilder MapGetBook(this IEndpointRouteBuilder app)
		{
			app.MapGet(ApiEndpoints.Books.Get, async (
					string idOrSlug, IBookService bookService,
					HttpContext context, CancellationToken token) =>
			{
				var userId = context.GetUserId();

				var book = Guid.TryParse(idOrSlug, out var id)
					? await bookService.GetByIdAsync(id, userId, token)
					: await bookService.GetBySlugAsync(idOrSlug, userId, token);

				if (book is null)
				{
					return Results.NotFound();
				}

				var response = book.MapToResponse();
				
				return TypedResults.Ok(response);
			})
				.WithName(Name)
				.Produces<BookResponse>(StatusCodes.Status200OK)
				.Produces(StatusCodes.Status404NotFound)
				.CacheOutput("BookCache");

			return app;
		}
	}
}
