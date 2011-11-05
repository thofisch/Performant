using System.Data;

namespace ConsoleApplication1.Data
{
	public interface IDatabase
	{
		IDbConnection CreateConnection();
	}
}