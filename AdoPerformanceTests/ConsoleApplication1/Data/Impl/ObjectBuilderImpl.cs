using System.Collections.Generic;
using System.Data;

namespace ConsoleApplication1.Data.Impl
{
	public class ObjectBuilderImpl<T> : IObjectBuilder<T>
	{
		private readonly IDataMapper<T> _dataMapper;

		public ObjectBuilderImpl(IDataMapper<T> dataMapper)
		{
			_dataMapper = dataMapper;
		}

		public T BuildObject(IDataReader reader)
		{
			return _dataMapper.Map(reader);
		}

		public IList<T> BuildObjectList(IDataReader reader, IList<T> list)
		{
			if( list==null )
			{
				list = new List<T>();
			}

			if( reader!=null )
			{
				while( reader.Read() )
				{
					var instance = BuildObject(reader);
					list.Add(instance);
				}
			}
			return list;
		}

		public IList<T> BuildObjectList(IDataReader reader)
		{
			return BuildObjectList(reader, null);
		}
	}
}