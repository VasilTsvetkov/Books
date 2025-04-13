using Microsoft.Data.SqlClient;
using System.Data;

namespace Books.Application.Database
{
	public interface IDbConnectionFactory
	{
		Task<IDbConnection> CreateConnectionAsync();
	}

	public class MssqlConnectionFactory(string connectionString) : IDbConnectionFactory
	{
		public async Task<IDbConnection> CreateConnectionAsync()
		{
			var connection = new SqlConnection(connectionString);

			await connection.OpenAsync();
			return connection;
		}
	}
}
