namespace Books.Api.Health
{
	using Application.Database;
	using Microsoft.Extensions.Diagnostics.HealthChecks;

	public class DatabaseHealthCheck(IDbConnectionFactory dbConnectionFactory, ILogger<DatabaseHealthCheck> logger) : IHealthCheck
	{
		public const string Name = "Database";

		public async Task<HealthCheckResult> CheckHealthAsync(
			HealthCheckContext context, CancellationToken cancellationToken = new())
		{
			try
			{
				_ = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);
				return HealthCheckResult.Healthy();
			}
			catch (Exception e)
			{
				const string errorMessage = "Database is unhealthy";
				logger.LogError(e, errorMessage);
				return HealthCheckResult.Unhealthy(errorMessage, e);
			}
		}
	}
}