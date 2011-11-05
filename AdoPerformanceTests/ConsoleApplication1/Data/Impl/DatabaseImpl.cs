using System.Data;
using System.Data.SqlClient;

namespace ConsoleApplication1.Data.Impl
{
	public class DatabaseImpl : IDatabase
	{
		private readonly string _connectionString;

		public DatabaseImpl(string connectionString)
		{
			_connectionString = connectionString;
		}

		public IDbConnection CreateConnection()
		{
			var connection = new SqlConnection(_connectionString);
			connection.Open();
			return connection;
		}
	}
}