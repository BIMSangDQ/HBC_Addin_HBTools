using System.Windows;
using System.Windows.Controls;
using SingleData;

namespace Property.WPF
{
	public partial class BaseAttachedProperty
	{
		public static readonly DependencyProperty CloseClickedProperty = DependencyProperty.RegisterAttached(
			"CloseClicked", typeof(string), typeof(BaseAttachedProperty), new PropertyMetadata(null, new PropertyChangedCallback(OnCloseClickedPropertyChanged)));

		public static string GetCloseClickedProperty(DependencyObject obj)
		{
			return (string)obj.GetValue(CloseClickedProperty);
		}
		public static void SetCloseClickedProperty(DependencyObject obj, string value)
		{
			obj.SetValue(CloseClickedProperty, value);
		}
		private static void OnCloseClickedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Button btn = d as Button;
			if (btn == null) return;

			btn.Click -= CloseClicked;
			if ((string)e.NewValue != null)
			{
				btn.Click += CloseClicked;
			}
		}

		private static void CloseClicked(object sender, RoutedEventArgs e)
		{
			RevitModelData.Instance.Form.Close();
		}
	}
}
