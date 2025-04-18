﻿using Books.Application.Models;

namespace Books.Application.Services
{
	public interface IBookService
	{
		Task<bool> CreateAsync(Book book, CancellationToken token = default);

		Task<Book?> GetByIdAsync(Guid id, CancellationToken token = default);

		Task<Book?> GetBySlugAsync(string slug, CancellationToken token = default);

		Task<IEnumerable<Book>> GetAllAsync(CancellationToken token = default);

		Task<Book?> UpdateAsync(Book book, CancellationToken token = default);

		Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default);
	}
}
