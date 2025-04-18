using Microsoft.Data.SqlClient;
using System.Data;

namespace Books.Application.Database
{
	public interface IDbConnectionFactory
	{
		Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default);
	}

	public class MssqlConnectionFactory(string connectionString) : IDbConnectionFactory
	{
		public async Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default)
		{
			var connection = new SqlConnection(connectionString);

			await connection.OpenAsync(token);
			return connection;
		}
	}
}
