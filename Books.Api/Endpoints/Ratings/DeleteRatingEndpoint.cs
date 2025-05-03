namespace Books.Api.Endpoints.Ratings
{
	using Application.Services;
	using Auth;

	public static class DeleteRatingEndpoint
	{
		public const string Name = "DeleteRating";

		public static IEndpointRouteBuilder MapDeleteRating(this IEndpointRouteBuilder app)
		{
			app.MapDelete(ApiEndpoints.Books.DeleteRating,
					async (Guid id, HttpContext context, IRatingService ratingService,
						CancellationToken token) =>
					{
						var userId = context.GetUserId();
						var result = await ratingService.DeleteRatingAsync(id, userId!.Value, token);

						return result ? Results.Ok() : Results.NotFound();
					})
				.WithName(Name)
				.Produces(StatusCodes.Status200OK)
				.Produces(StatusCodes.Status404NotFound)
				.RequireAuthorization();

			return app;
		}
	}
}