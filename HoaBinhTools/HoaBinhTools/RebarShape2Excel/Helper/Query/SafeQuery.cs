using System;
using System.Data.SqlClient;
using System.Reflection;

namespace Helper
{
	public static class SafeQuery
	{
		public static int? GetSafeInt(this SqlDataReader reader, int index)
		{
			if (reader.GetValue(index) == DBNull.Value) return null;
			return reader.GetInt32(index);
		}
		public static string GetSafeString(this SqlDataReader reader, int index)
		{
			if (reader.GetValue(index) == DBNull.Value) return null;
			return reader.GetString(index);
		}
		public static bool IsDBNull(this SqlDataReader reader, int index)
		{
			if (reader.GetValue(index) == DBNull.Value) return true;
			return false;
		}
		public static void SetValue(this PropertyInfo propInfo, object obj, SqlDataReader reader, int index)
		{
			if (reader.IsDBNull(index))
			{
				propInfo.SetValue(obj, null); return;
			}

			if (propInfo.PropertyType == typeof(int) || propInfo.PropertyType == typeof(int?))
			{
				propInfo.SetValue(obj, reader.GetInt32(index));
			}
			else if (propInfo.PropertyType == typeof(double))
			{
				propInfo.SetValue(obj, reader.GetDouble(index));
			}
			else if (propInfo.PropertyType == typeof(string))
			{
				propInfo.SetValue(obj, reader.GetSafeString(index));
			}
			else if (propInfo.PropertyType == typeof(bool))
			{
				propInfo.SetValue(obj, reader.GetBoolean(index));
			}
			else if (propInfo.PropertyType == typeof(DateTime))
			{
				propInfo.SetValue(obj, reader.GetDateTime(index));
			}
			else if (propInfo.PropertyType == typeof(decimal))
			{
				propInfo.SetValue(obj, reader.GetDecimal(index));
			}
			else if (propInfo.PropertyType == typeof(byte[]))
			{
				propInfo.SetValue(obj, (byte[])reader[index]);
			}
		}
	}
}
