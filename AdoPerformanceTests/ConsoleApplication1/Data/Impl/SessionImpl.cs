using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConsoleApplication1.Data.Impl
{
	public class SessionImpl : Disposable, ISession
	{
		private readonly ConnectionManager _connectionManager;

		public SessionImpl(IDatabase database)
		{
			_connectionManager = new ConnectionManager(database);
		}

		public ITransaction BeginTransaction()
		{
			return _connectionManager.BeginTransaction();
		}

		public ITransaction BeginTransaction(IsolationLevel isolationLevel)
		{
			return _connectionManager.BeginTransaction(isolationLevel);
		}

		public T Get<T>(object id)
		{
			var @params = new Dictionary<string, object>
			{
				{"@id", id}
			};
			return Find<T>(string.Format("select * from [{0}] where id = @id", typeof(T).Name), @params).SingleOrDefault();
		}

		public IList<T> Find<T>(string query, IDictionary<string, object> values)
		{
			using(var command = CreateCommand(CommandType.Text, query, values))
			{
				var items = new List<T>();
				using(var reader = command.ExecuteReader())
				{
					while( reader.Read() )
					{
						items.Add(ObjectLoader.Load<T>(reader));
					}
					reader.Close();
				}
				return items;
			}
		}

		public IList<T> FindAll<T>()
		{
			using(var command = CreateCommand(CommandType.Text, string.Format("select * from [{0}]", typeof(T).Name)))
			{
				var items = new List<T>();
				using(var reader = command.ExecuteReader())
				{
					while( reader.Read() )
					{
						items.Add(ObjectLoader.Load<T>(reader));
					}
					reader.Close();
				}
				return items;
			}
		}

		public IDbCommand CreateCommand(CommandType commandType, string commandText)
		{
			var command = _connectionManager.CreateCommand();
			command.CommandType = commandType;
			command.CommandText = commandText;
			return command;
		}

		public IDbCommand CreateCommand(CommandType commandType, string commandText, IDictionary<string, object> values)
		{
			var command = CreateCommand(commandType, commandText);

			switch( commandType )
			{
				case CommandType.Text:
					var regex = new Regex(@"@[^\s]+");
					var matches = regex.Matches(commandText);
					foreach(Match match in matches)
					{
						AddParameter(command, match.Value, values[match.Value]);
					}
					break;
				case CommandType.StoredProcedure:
					foreach(var pair in values)
					{
						AddParameter(command, pair.Key, pair.Value);
					}
					break;
			}

			return command;
		}

		public void AddParameter(IDbCommand command, string name, object value)
		{
			var parameter = command.CreateParameter();
			parameter.ParameterName = name.StartsWith("@") ? name : "@" + name;
			parameter.Value = value;

			if( value==null )
			{
				parameter.Value = DBNull.Value; // NOTE: Type is not set - defaults to string. Conversion is handled by the parameter implementation
			}
			else if( value.GetType()==typeof(int) )
			{
				parameter.DbType = DbType.Int32;
			}
			else if( value.GetType()==typeof(bool) )
			{
				parameter.DbType = DbType.Boolean;
			}
			else if( value.GetType()==typeof(DateTime) )
			{
				parameter.DbType = DbType.DateTime;
			}
			else
			{
				parameter.DbType = DbType.String;
			}

			command.Parameters.Add(parameter);
		}

		public void Create<T>(T item)
		{
			var paramCollection = new Dictionary<string, object>();
			var properties = typeof(T).GetProperties();
			foreach(var property in properties)
			{
				if( !property.CanWrite )
				{
					continue;
				}
				var propertyName = property.Name;
				var queryPropertyName = propertyName.Substring(0, 1).ToLowerInvariant() + propertyName.Substring(1);
				paramCollection.Add(queryPropertyName, property.GetValue(item, null));
			}

			using(var command = CreateCommand(CommandType.StoredProcedure, typeof(T).Name + "Insert", paramCollection))
			{
				var obj = command.ExecuteScalar();
				if( obj==null )
				{
					return;
				}
				var idProperty = typeof(T).GetProperty("Id");
				idProperty.SetValue(item, Convert.ToInt32(obj), null);
			}
		}

		public void Update<T>(T item)
		{
			var paramCollection = new Dictionary<string, object>();
			var properties = typeof(T).GetProperties();
			foreach(var property in properties)
			{
				if( !property.CanWrite )
				{
					continue;
				}
				var propertyName = property.Name;
				var queryPropertyName = propertyName.Substring(0, 1).ToLowerInvariant() + propertyName.Substring(1);
				paramCollection.Add(queryPropertyName, property.GetValue(item, null));
			}
			using(var command = CreateCommand(CommandType.StoredProcedure, typeof(T).Name + "Update", paramCollection))
			{
				command.ExecuteNonQuery();
			}
		}

		public void Delete<T>(object id)
		{
			var paramCollection = new Dictionary<string, object>
			{
				{"@id", id}
			};
			using(var command = CreateCommand(CommandType.StoredProcedure, typeof(T).Name + "Delete", paramCollection))
			{
				command.ExecuteNonQuery();
			}
		}

		protected override void DisposeManagedResources()
		{
			_connectionManager.Dispose();
		}
	}
}