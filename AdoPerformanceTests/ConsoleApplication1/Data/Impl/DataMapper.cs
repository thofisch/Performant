using System;
using System.Data;
using System.Linq;

namespace ConsoleApplication1.Data.Impl
{
	public static class DataMapper
	{
		public static bool? GetBool(IDataRecord record, string column)
		{
			if (record[column] == DBNull.Value)
			{
				return null;
			}

			var value = record[column].ToString();
			if (string.IsNullOrEmpty(value))
			{
				return null;
			}

			value = value.ToLowerInvariant();
			var @true = new[] { "yes", "true", "1" };
			var @false = new[] { "no", "false", "0" };

			if (@true.Contains(value))
			{
				return true;
			}
			if (@false.Contains(value))
			{
				return false;
			}

			bool x;
			if (bool.TryParse(value, out x))
			{
				return x;
			}

			return null;
		}

		public static int? GetInt(IDataRecord record, string column)
		{
			if (record[column] == DBNull.Value)
			{
				return null;
			}

			var value = record[column].ToString();
			if (string.IsNullOrEmpty(value))
			{
				return null;
			}

			int x;
			if (int.TryParse(value, out x))
			{
				return x;
			}

			return null;
		}

		public static string GetString(IDataRecord record, string column)
		{
			if (record[column] == DBNull.Value)
			{
				return null;
			}

			var value = (string)record[column];
			return value;
		}

		public static DateTime? GetDate(IDataRecord record, string column)
		{
			if (record[column] == DBNull.Value)
			{
				return null;
			}

			var value = record[column].ToString();
			if (string.IsNullOrEmpty(value))
			{
				return null;
			}

			DateTime x;
			if (DateTime.TryParse(value, out x))
			{
				return x;
			}

			return null;
		}

		public static char? GetChar(IDataRecord record, string column)
		{
			if (record[column] == DBNull.Value)
			{
				return null;
			}

			var value = record[column].ToString();
			if (string.IsNullOrEmpty(value))
			{
				return null;
			}

			char x;
			if (char.TryParse(value, out x))
			{
				return x;
			}

			return null;
		}
	}
}
