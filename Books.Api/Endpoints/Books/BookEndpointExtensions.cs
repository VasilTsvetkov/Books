namespace Books.Api.Endpoints.Books
{
	public static class BookEndpointExtensions
	{
		public static IEndpointRouteBuilder MapBookEndpoints(this IEndpointRouteBuilder app)
		{
			app.MapGetBook();
			app.MapCreateBook();
			app.MapGetAllBooks();
			app.MapUpdateBook();
			app.MapDeleteBook();

			return app;
		}
	}
}