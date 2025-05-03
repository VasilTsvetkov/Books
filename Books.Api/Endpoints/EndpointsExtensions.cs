namespace Books.Api.Endpoints
{
	using Books;
	using Ratings;

	public static class EndpointsExtensions
	{
		public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder app)
		{
			app.MapBookEndpoints();
			app.MapRatingEndpoints();
			return app;
		}
	}
}