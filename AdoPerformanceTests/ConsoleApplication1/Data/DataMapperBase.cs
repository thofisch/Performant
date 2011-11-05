using System;
using System.Data;

namespace ConsoleApplication1.Data
{
	public abstract class DataMapperBase<T> : IDataMapper<T>
	{
		private IDataReader _reader;

		T IDataMapper<T>.Map(IDataReader reader)
		{
			_reader = reader;
			return Map(reader);
		}

		public abstract T Map(IDataReader reader);

		protected virtual string GetString(string fieldName)
		{
			var oridinal = GetOrdinal(fieldName);
			if( _reader.IsDBNull(oridinal) )
			{
				return string.Empty;
			}
			return GetValue<string>(fieldName, _reader.GetString);
		}

		#region GetInt32
		public int GetInt32(string fieldName)
		{
			return GetValue<int>(fieldName, _reader.GetInt32);
		}

		public int GetInt32OrDefault(string fieldName, int defaultValue)
		{
			return GetValueOrDefault(fieldName, _reader.GetInt32, defaultValue);
		}

		public int? GetNullableInt32(string fieldName)
		{
			return GetNullable<int>(fieldName, _reader.GetInt32);
		}
		#endregion

		#region GetDouble
		public double GetDouble(string fieldName)
		{
			return GetValue<double>(fieldName, _reader.GetDouble);
		}

		public double GetDoubleOrDefault(string fieldName, double defaultValue)
		{
			return GetValueOrDefault(fieldName, _reader.GetDouble, defaultValue);
		}

		public double? GetNullableDouble(string fieldName)
		{
			return GetNullable<double>(fieldName, _reader.GetDouble);
		}
		#endregion

		#region GetBool
		public bool GetBool(string fieldName)
		{
			return GetValue<bool>(fieldName, _reader.GetBoolean);
		}

		public bool GetBoolOrDefault(string fieldName, bool defaultValue)
		{
			return GetValueOrDefault(fieldName, _reader.GetBoolean, defaultValue);
		}

		public bool? GetNullableBool(string fieldName)
		{
			return GetNullable<bool>(fieldName, _reader.GetBoolean);
		}
		#endregion

		#region GetDateTime
		public DateTime GetDateTime(string fieldName)
		{
			return GetValue<DateTime>(fieldName, _reader.GetDateTime);
		}

		public DateTime GetDateTimeOrDefault(string fieldName, DateTime defaultValue)
		{
			return GetValueOrDefault(fieldName, _reader.GetDateTime, defaultValue);
		}

		public DateTime? GetNullableDateTime(string fieldName)
		{
			return GetNullable<DateTime>(fieldName, _reader.GetDateTime);
		}
		#endregion

		#region GetGuid
		public Guid GetGuid(string fieldName)
		{
			return GetValue<Guid>(fieldName, _reader.GetGuid);
		}
		#endregion

		#region GetEnum
		public S GetEnum<S>(string fieldName)
		{
			var enumType = typeof(S);
			if( !enumType.IsEnum )
			{
				throw new ArgumentException(enumType.FullName + " is not an Enum");
			}

			return (S) Enum.Parse(typeof(T), GetString(fieldName));
		}
		#endregion

		#region Helper Methods
		private S GetValue<S>(string fieldName, Func<int, S> parser)
		{
			return parser(GetOrdinal(fieldName));
		}

		private int GetOrdinal(string fieldName)
		{
			return _reader.GetOrdinal(fieldName);
		}

		private S GetValueOrDefault<S>(string fieldName, Func<int, S> parser, S defaultValue)
		{
			var ordinal = GetOrdinal(fieldName);
			if( _reader.IsDBNull(ordinal) )
			{
				return defaultValue;
			}
			return parser(ordinal);
		}

		private S? GetNullable<S>(string fieldName, Func<int, S> parser) where S : struct
		{
			var ordinal = GetOrdinal(fieldName);
			if( _reader.IsDBNull(ordinal) )
			{
				return null;
			}
			return parser(ordinal);
		}
		#endregion
	}
}