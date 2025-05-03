using Books.Api.Endpoints.Books;
using Books.Api.Endpoints.Ratings;

namespace Books.Api.Endpoints
{
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
