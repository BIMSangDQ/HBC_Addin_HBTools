using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using SingleData;

namespace Property.WPF
{
	public partial class BaseAttachedProperty
	{
		public static readonly DependencyProperty AddRightToLeftClickedProperty = DependencyProperty.RegisterAttached(
			"AddRightToLeftClicked", typeof(string), typeof(BaseAttachedProperty), new PropertyMetadata(null, new PropertyChangedCallback(OnAddRightToLeftClickedPropertyChanged)));

		public static string GetAddRightToLeftClickedProperty(DependencyObject obj)
		{
			return (string)obj.GetValue(AddRightToLeftClickedProperty);
		}
		public static void SetAddRightToLeftClickedProperty(DependencyObject obj, string value)
		{
			obj.SetValue(AddRightToLeftClickedProperty, value);
		}
		private static void OnAddRightToLeftClickedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Button btn = d as Button;
			if (btn == null) return;

			btn.Click -= AddRightToLeftClicked;
			if ((string)e.NewValue != null)
			{
				btn.Click += AddRightToLeftClicked;
			}
		}

		private static void AddRightToLeftClicked(object sender, RoutedEventArgs e)
		{
			var wpfData = WPFData.Instance;
			var viewSchedules = wpfData.ViewSchedules;
			var exportViewSchedules = wpfData.ExportViewSchedules;

			var lv = wpfData.ExportViewSchedulesListView;
			var currentViewSchedules = new List<Autodesk.Revit.DB.ViewSchedule>();
			foreach (var obj in lv.SelectedItems)
			{
				var viewSchedule = obj as Autodesk.Revit.DB.ViewSchedule;
				currentViewSchedules.Add(viewSchedule);
			}

			for (int i = 0; i < currentViewSchedules.Count; i++)
			{
				viewSchedules.Add(currentViewSchedules[i]);
				exportViewSchedules.Remove(currentViewSchedules[i]);
			}
		}
	}
}
