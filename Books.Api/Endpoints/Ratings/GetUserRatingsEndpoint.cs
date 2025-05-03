namespace Books.Api.Endpoints.Ratings
{
	using Application.Services;
	using Auth;
	using Contracts.Responses;
	using Mapping;

	public static class GetUserRatingsEndpoint
	{
		public const string Name = "GetUserRatings";

		public static IEndpointRouteBuilder MapGetUserRatings(this IEndpointRouteBuilder app)
		{
			app.MapGet(ApiEndpoints.Ratings.GetUserRatings,
					async (HttpContext context, IRatingService ratingService,
						CancellationToken token) =>
					{
						var userId = context.GetUserId();
						var ratings = await ratingService.GetRatingsForUserAsync(userId!.Value, token);
						return TypedResults.Ok(ratings.MapToResponse());
					})
				.WithName(Name)
				.Produces<BookRatingResponse>(StatusCodes.Status200OK)
				.RequireAuthorization();

			return app;
		}
	}
}