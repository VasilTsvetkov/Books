namespace Books.Api.Endpoints.Ratings
{
	public static class RatingEndpointExtensions
	{
		public static IEndpointRouteBuilder MapRatingEndpoints(this IEndpointRouteBuilder app)
		{
			app.MapRateBook();
			app.MapDeleteRating();
			app.MapGetUserRatings();

			return app;
		}
	}
}
