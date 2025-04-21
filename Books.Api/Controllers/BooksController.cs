using Books.Api.Auth;
using Books.Api.Mapping;
using Books.Application.Services;
using Books.Contracts.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Books.Api.Controllers
{
    [ApiController]
	public class BooksController(IBookService bookService) : ControllerBase
	{
		[Authorize(AuthConstants.TrustedMemberPolicyName)]
		[HttpPost(ApiEndpoints.Books.Create)]
		public async Task<IActionResult> Create([FromBody]UpsertBookRequest request, CancellationToken token)
		{
			var book = request.MapToBook();

			await bookService.CreateAsync(book, token);
			return CreatedAtAction(nameof(Get), new { idOrSlug = book.Id }, book.MapToResponse());
		}

		[HttpGet(ApiEndpoints.Books.Get)]
		public async Task<IActionResult> Get([FromRoute]string idOrSlug, CancellationToken token)
		{
			var userId = HttpContext.GetUserId();

			var book = Guid.TryParse(idOrSlug, out var id)
				? await bookService.GetByIdAsync(id, userId, token)
				: await bookService.GetBySlugAsync(idOrSlug, userId, token);

			if (book is null)
			{
				return NotFound();
			}

			return Ok(book.MapToResponse());
		}

		[HttpGet(ApiEndpoints.Books.GetAll)]
		public async Task<IActionResult> GetAll([FromQuery] GetAllBooksRequest request, CancellationToken token)
		{
			var userId = HttpContext.GetUserId();
			var options = request.MapToOptions()
				.WithUser(userId);
			var books = await bookService.GetAllAsync(options, token);
			var bookCount = await bookService.GetCountAsync(options.Title, options.YearOfRelease, token);

			return Ok(books.MapToResponse(request.Page, request.PageSize, bookCount));
		}

		[Authorize(AuthConstants.TrustedMemberPolicyName)]
		[HttpPut(ApiEndpoints.Books.Update)]
		public async Task<IActionResult> Update([FromRoute]Guid id, [FromBody]UpsertBookRequest request, CancellationToken token)
		{
			var userId = HttpContext.GetUserId();
			var book = request.MapToBook(id);

			var updatedBook = await bookService.UpdateAsync(book, userId, token);

			if (updatedBook is null)
			{
				return NotFound();
			}

			return Ok(updatedBook.MapToResponse());
		}

		[Authorize(AuthConstants.AdminUserPolicyName)]
		[HttpDelete(ApiEndpoints.Books.Delete)]
		public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
		{
			var deleted = await bookService.DeleteByIdAsync(id, token);

			if (!deleted)
			{
				return NotFound();
			}

			return Ok();
		}
	}
}
