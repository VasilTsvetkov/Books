using Books.Api.Mapping;
using Books.Application.Services;
using Books.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Books.Api.Controllers
{
	[ApiController]
	public class BooksController(IBookService bookService) : ControllerBase
	{
		[HttpPost(ApiEndpoints.Books.Create)]
		public async Task<IActionResult> Create([FromBody]UpsertBookRequest request)
		{
			var book = request.MapToBook();

			await bookService.CreateAsync(book);
			return CreatedAtAction(nameof(Get), new { idOrSlug = book.Id }, book.MapToResponse());
		}

		[HttpGet(ApiEndpoints.Books.Get)]
		public async Task<IActionResult> Get([FromRoute]string idOrSlug)
		{
			var book = Guid.TryParse(idOrSlug, out var id)
				? await bookService.GetByIdAsync(id)
				: await bookService.GetBySlugAsync(idOrSlug);

			if (book is null)
			{
				return NotFound();
			}

			return Ok(book.MapToResponse());
		}

		[HttpGet(ApiEndpoints.Books.GetAll)]
		public async Task<IActionResult> GetAll()
		{
			var books = await bookService.GetAllAsync();

			return Ok(books.MapToResponse());
		}

		[HttpPut(ApiEndpoints.Books.Update)]
		public async Task<IActionResult> Update([FromRoute]Guid id, [FromBody]UpsertBookRequest request)
		{
			var book = request.MapToBook(id);

			var updatedBook = await bookService.UpdateAsync(book);

			if (updatedBook is null)
			{
				return NotFound();
			}

			return Ok(updatedBook.MapToResponse());
		}

		[HttpDelete(ApiEndpoints.Books.Delete)]
		public async Task<IActionResult> Delete([FromRoute] Guid id)
		{
			var deleted = await bookService.DeleteByIdAsync(id);

			if (!deleted)
			{
				return NotFound();
			}

			return Ok();
		}

	}
}
