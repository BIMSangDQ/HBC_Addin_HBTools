using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HoaBinhTools.FramingInformation
{
	public class ConverterCollection<T>
	{
		public static List<T> ToList(ObservableCollection<T> Obser)
		{
			List<T> Lis = new List<T>();
			foreach (var ob in Obser)
			{
				Lis.Add(ob);
			}
			return (Lis);
		}

		public static ObservableCollection<T> ToObser(List<T> Lis)
		{
			ObservableCollection<T> Obsers = new ObservableCollection<T>();
			foreach (var Li in Lis)
			{
				Obsers.Add(Li);
			}
			return Obsers;
		}
	}

}
