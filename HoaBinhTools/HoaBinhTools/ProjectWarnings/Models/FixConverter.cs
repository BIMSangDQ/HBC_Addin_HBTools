using System;
using System.Windows.Data;

namespace HoaBinhTools.ProjectWarnings.Models
{
	public class FixConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{

			if (value as string == "")
			{
				return false;
			}
			return true;
		}


		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}

	}
}
