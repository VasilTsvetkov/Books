using Books.Api.Auth;
using Books.Application.Services;
using Microsoft.AspNetCore.OutputCaching;

namespace Books.Api.Endpoints.Books
{
	public static class DeleteBookEndpoint
	{
		public const string Name = "DeleteBook";

		public static IEndpointRouteBuilder MapDeleteBook(this IEndpointRouteBuilder app)
		{
			app.MapDelete(ApiEndpoints.Books.Delete, async (
					Guid id, IBookService bookService,
					IOutputCacheStore outputCacheStore, CancellationToken token) =>
			{
				var deleted = await bookService.DeleteByIdAsync(id, token);

				if (!deleted)
				{
					return Results.NotFound();
				}

				await outputCacheStore.EvictByTagAsync("books", token);
				return Results.Ok();
			})
				.WithName(Name)
				.Produces(StatusCodes.Status200OK)
				.Produces(StatusCodes.Status404NotFound)
				.RequireAuthorization(AuthConstants.AdminUserPolicyName);

			return app;
		}
	}
}
