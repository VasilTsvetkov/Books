using Books.Application.Models;

namespace Books.Application.Services
{
	public interface IBookService
	{
		Task<bool> CreateAsync(Book book, CancellationToken token = default);

		Task<Book?> GetByIdAsync(Guid bookId, Guid? userId = default, CancellationToken token = default);

		Task<Book?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default);

		Task<IEnumerable<Book>> GetAllAsync(GetAllBooksOptions options, CancellationToken token = default);

		Task<Book?> UpdateAsync(Book book, Guid? userId = default, CancellationToken token = default);

		Task<bool> DeleteByIdAsync(Guid bookId, CancellationToken token = default);
	}
}
