using Books.Application.Models;

namespace Books.Application.Repositories
{
	public class BookRepository : IBookRepository
	{
		private readonly List<Book> _books = new();

		public Task<bool> CreateAsync(Book book)
		{
			_books.Add(book);
			return Task.FromResult(true);
		}

		public Task<bool> DeleteByIdAsync(Guid id)
		{
			var booksRemovedCount = _books.RemoveAll(x => x.Id == id);
			return Task.FromResult(booksRemovedCount > 0);
		}

		public Task<IEnumerable<Book>> GetAllAsync()
			=> Task.FromResult(_books.AsEnumerable());

		public Task<Book?> GetByIdAsync(Guid id)
		{
			var book = _books.FirstOrDefault(x => x.Id == id);
			return Task.FromResult(book);
		}

		public Task<bool> UpdateAsync(Book book)
		{
			var bookIndex = _books.FindIndex(x => x.Id == book.Id);

			if (bookIndex == -1)
			{
				return Task.FromResult(false);
			}

			_books[bookIndex] = book;
			return Task.FromResult(true);
		}
	}
}
