using System.Collections.Generic;
using System.Data;

namespace ConsoleApplication1.Data
{
	public interface IObjectBuilder<T>
	{
		T BuildObject(IDataReader reader);
		IList<T> BuildObjectList(IDataReader reader, IList<T> list);
		IList<T> BuildObjectList(IDataReader reader);
	}
}