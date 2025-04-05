using Books.Application.Models;

namespace Books.Application.Repositories
{
	public interface IBookRepository
	{
		Task<bool> CreateAsync(Book book);

		Task<Book?> GetByIdAsync(Guid id);

		Task<IEnumerable<Book>> GetAllAsync();

		Task<bool> UpdateAsync(Book book);

		Task<bool> DeleteByIdAsync(Guid id);
	}
}
