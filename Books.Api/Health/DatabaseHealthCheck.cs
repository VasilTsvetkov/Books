﻿using Books.Application.Database;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Books.Api.Health;

public class DatabaseHealthCheck(IDbConnectionFactory dbConnectionFactory, ILogger<DatabaseHealthCheck> logger) : IHealthCheck
{
	public const string Name = "Database";

	private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;
	private readonly ILogger<DatabaseHealthCheck> _logger = logger;

	public async Task<HealthCheckResult> CheckHealthAsync(
		HealthCheckContext context, CancellationToken cancellationToken = new())
	{
		try
		{
			_ = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
			return HealthCheckResult.Healthy();
		}
		catch (Exception e)
		{
			const string errorMessage = "Database is unhealthy";
			_logger.LogError(e, errorMessage);
			return HealthCheckResult.Unhealthy(errorMessage, e);
		}
	}
}