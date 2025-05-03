namespace Books.Api.Endpoints.Ratings
{
	using Application.Services;
	using Auth;
	using Contracts.Requests;

	public static class RateBookEndpoint
	{
		public const string Name = "RateBook";

		public static IEndpointRouteBuilder MapRateBook(this IEndpointRouteBuilder app)
		{
			app.MapPut(ApiEndpoints.Books.Rate,
					async (Guid id, RateBookRequest request,
						HttpContext context, IRatingService ratingService,
						CancellationToken token) =>
					{
						var userId = context.GetUserId();
						var result = await ratingService.RateBookAsync(id, request.Rating, userId!.Value, token);
						
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