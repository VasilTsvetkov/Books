using Asp.Versioning;
using Books.Api.Auth;
using Books.Api.Mapping;
using Books.Application.Services;
using Books.Contracts.Requests;
using Books.Contracts.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Books.Api.Controllers
{
	[ApiController]
	[ApiVersion(1.0)]
	public class BooksController(IBookService bookService, IOutputCacheStore outputCacheStore) : ControllerBase
	{
		[Authorize(AuthConstants.TrustedMemberPolicyName)]
		[HttpPost(ApiEndpoints.Books.Create)]
		[ProducesResponseType(typeof(BookResponse), StatusCodes.Status201Created)]
		[ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Create([FromBody] UpsertBookRequest request, CancellationToken token)
		{
			var book = request.MapToBook();

			await bookService.CreateAsync(book, token);
			await outputCacheStore.EvictByTagAsync("books", token);
			return CreatedAtAction(nameof(GetV1), new { idOrSlug = book.Id }, book.MapToResponse());
		}

		[HttpGet(ApiEndpoints.Books.Get)]
		[OutputCache(PolicyName = "BookCache")]
		[ProducesResponseType(typeof(BookResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetV1([FromRoute] string idOrSlug, CancellationToken token)
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
		[OutputCache(PolicyName = "BookCache")]
		[ProducesResponseType(typeof(BooksResponse), StatusCodes.Status200OK)]
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
		[ProducesResponseType(typeof(BookResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpsertBookRequest request, CancellationToken token)
		{
			var userId = HttpContext.GetUserId();
			var book = request.MapToBook(id);

			var updatedBook = await bookService.UpdateAsync(book, userId, token);

			if (updatedBook is null)
			{
				return NotFound();
			}

			await outputCacheStore.EvictByTagAsync("books", token);
			return Ok(updatedBook.MapToResponse());
		}

		[Authorize(AuthConstants.AdminUserPolicyName)]
		[HttpDelete(ApiEndpoints.Books.Delete)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
		{
			var deleted = await bookService.DeleteByIdAsync(id, token);

			if (!deleted)
			{
				return NotFound();
			}

			await outputCacheStore.EvictByTagAsync("books", token);
			return Ok();
		}
	}
}