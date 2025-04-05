using Books.Application.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Books.Application
{
	public static class ApplicationServiceCollectionExtensions
	{
		public static IServiceCollection AddApplication(this IServiceCollection services)
		{
			services.AddSingleton<IBookRepository, BookRepository>();
			return services;
		}
	}
}
