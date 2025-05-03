namespace Books.Api.Endpoints.Books
{
	using Application.Services;
	using Auth;
	using Contracts.Requests;
	using Contracts.Responses;
	using Mapping;

	public static class GetAllBooksEndpoint
	{
		public const string Name = "GetBooks";

		public static IEndpointRouteBuilder MapGetAllBooks(this IEndpointRouteBuilder app)
		{
			app.MapGet(ApiEndpoints.Books.GetAll, async (
					[AsParameters] GetAllBooksRequest request, IBookService bookService,
					HttpContext context, CancellationToken token) =>
			{
				var userId = context.GetUserId();
				var options = request.MapToOptions()
					.WithUser(userId);
				var books = await bookService.GetAllAsync(options, token);
				var bookCount = await bookService.GetCountAsync(options.Title, options.YearOfRelease, token);
				var booksResponse = books.MapToResponse(
					request.Page.GetValueOrDefault(PagedRequest.DefaultPage),
					request.PageSize.GetValueOrDefault(PagedRequest.DefaultPageSize),
					bookCount);

				return TypedResults.Ok(booksResponse);
			})
				.WithName($"{Name}V1")
				.Produces<BooksResponse>(StatusCodes.Status200OK)
				.WithApiVersionSet(ApiVersioning.VersionSet)
				.HasApiVersion(1.0);

			app.MapGet(ApiEndpoints.Books.GetAll, async (
					[AsParameters] GetAllBooksRequest request, IBookService bookService,
					HttpContext context, CancellationToken token) =>
			{
				var userId = context.GetUserId();
				var options = request.MapToOptions()
					.WithUser(userId);
				var books = await bookService.GetAllAsync(options, token);
				var bookCount = await bookService.GetCountAsync(options.Title, options.YearOfRelease, token);
				var booksResponse = books.MapToResponse(
					request.Page.GetValueOrDefault(PagedRequest.DefaultPage),
					request.PageSize.GetValueOrDefault(PagedRequest.DefaultPageSize),
					bookCount);

				return TypedResults.Ok(booksResponse);
			})
				.WithName($"{Name}V2")
				.Produces<BooksResponse>(StatusCodes.Status200OK)
				.WithApiVersionSet(ApiVersioning.VersionSet)
				.HasApiVersion(2.0)
				.CacheOutput("BookCache");

			return app;
		}
	}
}