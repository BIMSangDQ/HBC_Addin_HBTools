using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Utility
{
	public static class BindingUtil
	{
		public static void BindingObject<T1, T2>(this T1 obj1, T2 obj2)
		{
			PropertyInfo prop2 = typeof(T1).GetProperty(typeof(T2).Name);
			prop2.SetValue(obj1, obj2);

			PropertyInfo prop1 = typeof(T2).GetProperty(typeof(T1).Name);
			prop1.SetValue(obj2, obj1);
		}
		public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> list)
		{
			return new ObservableCollection<T>(list);
		}
		public static List<PropertyInfo> GetNamedOrderProperties(this Type type, List<string> names)
		{
			return names.Select(x => type.GetProperty(x)).ToList();
		}
		public static T GetItemByPropertyValue<T>(this IEnumerable<T> list, Type baseType, string propertyName, string propertyValue)
		{
			return list.Where(x => baseType.GetProperty(propertyName).GetValue(x) as string == propertyValue).First();
		}

		public static T Clone<T>(this T obj)
		{
			if (obj == null) return default(T);
			FieldInfo[] fieldInfos = typeof(T).GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

			T obj2 = (T)Activator.CreateInstance(typeof(T));
			foreach (FieldInfo fi in fieldInfos)
			{
				Type type = fi.FieldType;
				if (type.IsValueType())
				{
					fi.SetValue(obj2, fi.GetValue(obj));
				}
				else
				{
					var objType = Convert.ChangeType(fi.GetValue(obj), type);

					var cloneMethod = typeof(BindingUtil).GetMethod("Clone");
					var cloneOfTypeMethod = cloneMethod.MakeGenericMethod(new[] { type });
					objType = cloneOfTypeMethod.Invoke(null, new object[] { objType });

					fi.SetValue(obj2, objType);
				}
			}
			return obj2;
		}
		public static bool IsValueType(this Type type)
		{
			return type.IsValueType || type.Equals(typeof(string));
		}
	}
}
