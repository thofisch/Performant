using System;
using System.Data;

namespace ConsoleApplication1.Data.Impl
{
	public static class ObjectLoader
	{
		public static T Load<T>(IDataRecord record)
		{
			var properties = typeof(T).GetProperties();
			var item = Activator.CreateInstance(typeof(T));
			foreach(var property in properties)
			{
				if( !property.CanWrite )
				{
					continue;
				}

				var propertyName = property.Name;
				var dataPropertyName = propertyName.Substring(0, 1).ToLowerInvariant() + propertyName.Substring(1);

				if( record[dataPropertyName]==DBNull.Value )
				{
					property.SetValue(item, null, null);
				}
				else
				{
					if( property.PropertyType==typeof(int) )
					{
						property.SetValue(item, (int) record[dataPropertyName], null);
					}
					else if( property.PropertyType==typeof(bool) )
					{
						property.SetValue(item, (bool) record[dataPropertyName], null);
					}
					else if( property.PropertyType==typeof(char) )
					{
						property.SetValue(item, (char) record[dataPropertyName], null);
					}
					else if( property.PropertyType==typeof(DateTime) )
					{
						property.SetValue(item, (DateTime) record[dataPropertyName], null);
					}
					else if( property.PropertyType==typeof(string) )
					{
						property.SetValue(item, DataMapper.GetString(record, dataPropertyName), null);
					}
					else if( property.PropertyType==typeof(int?) )
					{
						property.SetValue(item, DataMapper.GetInt(record, dataPropertyName), null);
					}
					else if( property.PropertyType==typeof(bool?) )
					{
						property.SetValue(item, DataMapper.GetBool(record, dataPropertyName), null);
					}
					else if( property.PropertyType==typeof(char?) )
					{
						property.SetValue(item, DataMapper.GetChar(record, dataPropertyName), null);
					}
					else if( property.PropertyType==typeof(DateTime?) )
					{
						property.SetValue(item, DataMapper.GetDate(record, dataPropertyName), null);
					}
					else
					{
						throw new InvalidCastException("Could not find a valid cast for property " + propertyName + " on business object " + typeof(T).Name + ".");
					}
				}
			}

			return (T) item;
		}
	}
}