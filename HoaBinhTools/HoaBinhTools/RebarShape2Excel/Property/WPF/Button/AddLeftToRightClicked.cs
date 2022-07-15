using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using SingleData;

namespace Property.WPF
{
	public partial class BaseAttachedProperty
	{
		public static readonly DependencyProperty AddLeftToRightClickedProperty = DependencyProperty.RegisterAttached(
			"AddLeftToRightClicked", typeof(string), typeof(BaseAttachedProperty), new PropertyMetadata(null, new PropertyChangedCallback(OnAddLeftToRightClickedPropertyChanged)));

		public static string GetAddLeftToRightClickedProperty(DependencyObject obj)
		{
			return (string)obj.GetValue(AddLeftToRightClickedProperty);
		}
		public static void SetAddLeftToRightClickedProperty(DependencyObject obj, string value)
		{
			obj.SetValue(AddLeftToRightClickedProperty, value);
		}
		private static void OnAddLeftToRightClickedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Button btn = d as Button;
			if (btn == null) return;

			btn.Click -= AddLeftToRightClicked;
			if ((string)e.NewValue != null)
			{
				btn.Click += AddLeftToRightClicked;
			}
		}

		private static void AddLeftToRightClicked(object sender, RoutedEventArgs e)
		{
			var wpfData = WPFData.Instance;
			var viewSchedules = wpfData.ViewSchedules;
			var exportViewSchedules = wpfData.ExportViewSchedules;

			var lv = wpfData.ViewSchedulesListView;
			var currentViewSchedules = new List<Autodesk.Revit.DB.ViewSchedule>();
			foreach (var obj in lv.SelectedItems)
			{
				var viewSchedule = obj as Autodesk.Revit.DB.ViewSchedule;
				currentViewSchedules.Add(viewSchedule);
			}

			for (int i = 0; i < currentViewSchedules.Count; i++)
			{
				exportViewSchedules.Add(currentViewSchedules[i]);
				viewSchedules.Remove(currentViewSchedules[i]);
			}
		}
	}
}
