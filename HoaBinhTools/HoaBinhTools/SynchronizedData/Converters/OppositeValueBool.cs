using System;
using System.Globalization;
using System.Windows.Data;

namespace HoaBinhTools.SynchronizedData.Converters
{
	public class OppositeValueBool : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool boo = (bool)value;
			return !boo;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
