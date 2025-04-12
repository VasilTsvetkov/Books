using Microsoft.Data.SqlClient;
using System.Data;

namespace Books.Application.Database
{
	public interface IDbConnectionFactory
	{
		Task<IDbConnection> CreateConnectionAsync();
	}

	public class MssqlConnectionFactory : IDbConnectionFactory
	{
		private readonly string _connectionString;

		public MssqlConnectionFactory(string connectionString)
		{
			_connectionString = connectionString;
		}

        public async Task<IDbConnection> CreateConnectionAsync()
		{
			var connection = new SqlConnection(_connectionString);

			await connection.OpenAsync();
			return connection;
		}
	}
}
