using System;
using System.Windows;
using System.Windows.Controls;

namespace HoaBinhTools.FramingInformation.Views
{
	/// <summary>
	/// Interaction logic for FramingInfoView.xaml
	/// </summary>
	public partial class FramingInfoView : Window
	{
		public FramingInfoView(object Fs)
		{
			this.DataContext = Fs;

			InitializeComponent();
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			this.Close();
		}

		private void StirrupChangeDiameter(object sender, SelectionChangedEventArgs e)
		{
			string name = ((System.Windows.FrameworkElement)sender).Name;

			string selectItem = ((System.Windows.Controls.Primitives.Selector)sender).SelectedItem.ToString();

			switch (name)
			{
				case "Stirrup1":
					this.Stirrup2.Text = selectItem;
					this.Stirrup3.Text = selectItem;
					break;
				case "Stirrup2":
					this.Stirrup1.Text = selectItem;
					this.Stirrup3.Text = selectItem;
					break;
				case "Stirrup3":
					this.Stirrup1.Text = selectItem;
					this.Stirrup2.Text = selectItem;
					break;
			}
		}

		private void KhoangRaiDai(object sender, TextChangedEventArgs e)
		{
			if (this.Kc2.Text.Length < 3)
			{
				this.Kc2.Text = ((System.Windows.Controls.TextBox)sender).Text.ToString();
			}

			if (this.Kc3.Text.Length < 3)
			{
				this.Kc3.Text = ((System.Windows.Controls.TextBox)sender).Text.ToString();
			}
		}

		private void SpanChange(object sender, RoutedEventArgs e)
		{

		}
	}
}
