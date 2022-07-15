using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using Utility;

namespace Helper
{
	public abstract class AQueryHelper
	{
		public abstract object GetObject(SqlDataReader reader, QueryEnum queryEnum);
	}
	public enum QueryEnum
	{
		Single, Multiple
	}

	public class QueryHelper<T> : AQueryHelper
	{
		private static AQueryHelper instance;
		public static AQueryHelper Instance
		{
			get
			{
				if (instance == null) instance = new QueryHelper<T>(); return instance;
			}
		}
		public override object GetObject(SqlDataReader reader, QueryEnum queryEnum)
		{
			switch (queryEnum)
			{
				case QueryEnum.Single:
					{
						T obj = (T)Activator.CreateInstance(typeof(T));
						List<string> propertyNames = typeof(T).GetProperty("PropertyNames").GetValue(obj) as List<string>;
						List<PropertyInfo> props = typeof(T).GetNamedOrderProperties(propertyNames);
						while (reader.Read())
						{
							for (int i = 0; i < props.Count; i++)
							{
								props[i].SetValue(obj, reader, i);
							}
						}
						return obj;
					}
				case QueryEnum.Multiple:
					{
						List<T> objs = new List<T>();
						T obj = (T)Activator.CreateInstance(typeof(T));
						List<string> propertyNames = typeof(T).GetProperty("PropertyNames").GetValue(obj) as List<string>;
						List<PropertyInfo> props = typeof(T).GetNamedOrderProperties(propertyNames);
						while (reader.Read())
						{
							obj = (T)Activator.CreateInstance(typeof(T));
							for (int i = 0; i < props.Count; i++)
							{
								props[i].SetValue(obj, reader, i);
							}

							objs.Add(obj);
						}
						return objs;
					}
			}
			return new List<T>();
		}
	}
}
