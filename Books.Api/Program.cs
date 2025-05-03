namespace Books.Api
{
	using Application;
	using Application.Database;
	using Asp.Versioning;
	using Auth;
	using Endpoints;
	using Health;
	using Mapping;
	using Microsoft.AspNetCore.Authentication.JwtBearer;
	using Microsoft.Extensions.Options;
	using Microsoft.IdentityModel.Tokens;
	using Swagger;
	using Swashbuckle.AspNetCore.SwaggerGen;
	using System.Text;

	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			var config = builder.Configuration;

			builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					IssuerSigningKey = new SymmetricSecurityKey(
						Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
					ValidateIssuerSigningKey = true,
					ValidateLifetime = true,
					ValidIssuer = config["Jwt:Issuer"]!,
					ValidAudience = config["Jwt:Audience"]!,
					ValidateIssuer = true,
					ValidateAudience = true
				};
			});

			builder.Services.AddAuthorization(options =>
			{
				options.AddPolicy(AuthConstants.AdminUserPolicyName,
					p => p.RequireClaim(AuthConstants.AdminUserClaimName, "true"));

				options.AddPolicy(AuthConstants.TrustedMemberPolicyName,
					p => p.RequireAssertion(c => 
					c.User.HasClaim(m => m is { Type: AuthConstants.AdminUserClaimName, Value: "true" }) ||
					c.User.HasClaim(m => m is { Type: AuthConstants.TrustedMemberClaimName, Value: "true" })));
			});

			builder.Services.AddApiVersioning(options =>
			{
				options.DefaultApiVersion = new ApiVersion(1.0);
				options.AssumeDefaultVersionWhenUnspecified = true;
				options.ReportApiVersions = true;
				options.ApiVersionReader = new MediaTypeApiVersionReader("api-version");
			}).AddApiExplorer();

			builder.Services.AddEndpointsApiExplorer();

			builder.Services.AddOutputCache(x =>
			{
				x.AddBasePolicy(c => c.Cache());
				x.AddPolicy("BookCache", c =>
					c.Cache()
					.Expire(TimeSpan.FromMinutes(1))
					.SetVaryByQuery(new[] { "title", "year", "sortBy", "page", "pageSize" })
					.Tag("books"));
			});

			builder.Services.AddHealthChecks()
				.AddCheck<DatabaseHealthCheck>(DatabaseHealthCheck.Name);

			builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
			builder.Services.AddSwaggerGen(options => options.OperationFilter<SwaggerDefaultValues>());

			builder.Services.AddApplication();
			builder.Services.AddDatabase(config["Database:ConnectionString"]!);

			var app = builder.Build();

			app.CreateApiVersionSet();

			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI(x =>
				{
					foreach (var description in app.DescribeApiVersions())
					{
						x.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
							description.GroupName);
					}
				});
			}

			app.MapHealthChecks("_health");

			app.UseHttpsRedirection();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseOutputCache();

			app.UseMiddleware<ValidationMappingMiddleware>();
			app.MapApiEndpoints();

			var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
			await dbInitializer.InitializeAsync();

			app.Run();
		}
	}
}