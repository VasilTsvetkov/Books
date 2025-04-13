using Books.Application.Models;
using Books.Application.Repositories;

namespace Books.Application.Services
{
	public class BookService(IBookRepository bookRepository) : IBookService
	{
		public Task<bool> CreateAsync(Book book)
			=> bookRepository.CreateAsync(book);

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
