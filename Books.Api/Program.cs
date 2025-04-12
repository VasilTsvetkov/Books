using Books.Application;
using Books.Application.Database;

namespace Books.Api
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			var config = builder.Configuration;

			builder.Services.AddControllers();
			builder.Services.AddApplication();
			builder.Services.AddDatabase(config["Database:ConnectionString"]!);

			var app = builder.Build();

			app.UseHttpsRedirection();

			app.UseAuthorization();


			app.MapControllers();

			var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
			await dbInitializer.InitializeAsync();

			app.Run();
		}
	}
}
