using Books.Application.Models;

namespace Books.Application.Services
{
	public interface IBookService
	{
		Task<bool> CreateAsync(Book book);

		Task<Book?> GetByIdAsync(Guid id);

		Task<Book?> GetBySlugAsync(string slug);

		Task<IEnumerable<Book>> GetAllAsync();

		Task<Book?> UpdateAsync(Book book);

		Task<bool> DeleteByIdAsync(Guid id);
	}
}
