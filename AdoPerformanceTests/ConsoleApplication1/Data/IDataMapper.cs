using System.Data;

namespace ConsoleApplication1.Data
{
	public interface IDataMapper<T>
	{
		T Map(IDataReader reader);
	}
}