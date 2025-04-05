using Books.Api.Mapping;
using Books.Application.Models;
using Books.Application.Repositories;
using Books.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Books.Api.Controllers
{
	[ApiController]
	public class BooksController(IBookRepository bookRepository) : ControllerBase
	{
		[HttpPost(ApiEndpoints.Books.Create)]
		public async Task<IActionResult> Create([FromBody]UpsertBookRequest request)
		{
			var book = request.MapToBook();

			await bookRepository.CreateAsync(book);
			return CreatedAtAction(nameof(Get), new { id  = book.Id }, book.MapToResponse());
		}

		[HttpGet(ApiEndpoints.Books.Get)]
		public async Task<IActionResult> Get([FromRoute]Guid id)
		{
			var book = await bookRepository.GetByIdAsync(id);

			if (book == null)
			{
				return NotFound();
			}

			return Ok(book.MapToResponse());
		}

		[HttpGet(ApiEndpoints.Books.GetAll)]
		public async Task<IActionResult> GetAll()
		{
			var books = await bookRepository.GetAllAsync();

			return Ok(books.MapToResponse());
		}

		[HttpPut(ApiEndpoints.Books.Update)]
		public async Task<IActionResult> Update([FromRoute]Guid id, [FromBody]UpsertBookRequest request)
		{
			var book = request.MapToBook(id);

			var updated = await bookRepository.UpdateAsync(book);

			if (!updated)
			{
				return NotFound();
			}

			return Ok(book.MapToResponse());
		}

		[HttpDelete(ApiEndpoints.Books.Delete)]
		public async Task<IActionResult> Delete([FromRoute] Guid id)
		{
			var deleted = await bookRepository.DeleteByIdAsync(id);

			if (!deleted)
			{
				return NotFound();
			}

			return Ok();
		}

	}
}
