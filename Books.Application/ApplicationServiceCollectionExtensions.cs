using Books.Application.Database;
using Books.Application.Repositories;
using Books.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Books.Application
{
	public static class ApplicationServiceCollectionExtensions
	{
		public static IServiceCollection AddApplication(this IServiceCollection services)
		{
			services.AddSingleton<IBookRepository, BookRepository>();
			services.AddSingleton<IRatingRepository, RatingRepository>();
			services.AddSingleton<IBookService, BookService>();
			services.AddValidatorsFromAssemblyContaining<IApplicationMarker>(ServiceLifetime.Singleton);
			return services;
		}

		public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
		{
			services.AddSingleton<IDbConnectionFactory>(_ => new MssqlConnectionFactory(connectionString));
			services.AddSingleton<DbInitializer>();
			return services;
		}
	}
}
