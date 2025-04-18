using Books.Application.Models;
using Books.Application.Repositories;
using FluentValidation;

namespace Books.Application.Services
{
	public class BookService(IBookRepository bookRepository, IValidator<Book> bookValidator) : IBookService
	{
		public async Task<bool> CreateAsync(Book book, CancellationToken token = default)
		{
			await bookValidator.ValidateAndThrowAsync(book, token);

			return await bookRepository.CreateAsync(book, token);
		}

		public Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
			=> bookRepository.DeleteByIdAsync(id, token);

		public Task<IEnumerable<Book>> GetAllAsync(CancellationToken token = default)
			=> bookRepository.GetAllAsync(token);

		public Task<Book?> GetByIdAsync(Guid id, CancellationToken token = default)
			=> bookRepository.GetByIdAsync(id, token);

		public Task<Book?> GetBySlugAsync(string slug, CancellationToken token = default)
			=> bookRepository.GetBySlugAsync(slug, token);

		public async Task<Book?> UpdateAsync(Book book, CancellationToken token = default)
		{
			await bookValidator.ValidateAndThrowAsync(book, token);

			var bookExists = await bookRepository.ExistsByIdAsync(book.Id, token);

			if (!bookExists)
			{
				return null;
			}

			await bookRepository.UpdateAsync(book, token);
			return book;
		}
	}
}