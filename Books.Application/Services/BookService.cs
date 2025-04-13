using Books.Application.Models;
using Books.Application.Repositories;
using FluentValidation;

namespace Books.Application.Services
{
	public class BookService(IBookRepository bookRepository, IValidator<Book> bookValidator) : IBookService
	{
		public async Task<bool> CreateAsync(Book book)
		{
			await bookValidator.ValidateAndThrowAsync(book);

			return await bookRepository.CreateAsync(book);
		}

		public Task<bool> DeleteByIdAsync(Guid id)
			=> bookRepository.DeleteByIdAsync(id);

		public Task<IEnumerable<Book>> GetAllAsync()
			=> bookRepository.GetAllAsync();

		public Task<Book?> GetByIdAsync(Guid id)
			=> bookRepository.GetByIdAsync(id);

		public Task<Book?> GetBySlugAsync(string slug)
			=> bookRepository.GetBySlugAsync(slug);

		public async Task<Book?> UpdateAsync(Book book)
		{
			await bookValidator.ValidateAndThrowAsync(book);

			var bookExists = await bookRepository.ExistsByIdAsync(book.Id);

			if (!bookExists)
			{
				return null;
			}

			await bookRepository.UpdateAsync(book);
			return book;
		}
	}
}