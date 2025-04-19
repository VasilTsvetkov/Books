using Books.Api.Mapping;
using Books.Application;
using Books.Application.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Books.Api
{
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

			builder.Services.AddControllers();
			builder.Services.AddApplication();
			builder.Services.AddDatabase(config["Database:ConnectionString"]!);

			var app = builder.Build();

			app.UseHttpsRedirection();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseMiddleware<ValidationMappingMiddleware>();
			app.MapControllers();

			var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
			await dbInitializer.InitializeAsync();

			app.Run();
		}
	}
}
