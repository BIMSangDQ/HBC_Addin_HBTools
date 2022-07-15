using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Utils
{
	public abstract class ViewModelBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public static event PropertyChangedEventHandler StaticPropertyChanged;

		public static void OnStatisPropertyChanged([CallerMemberName] string propertyName = null)
		{
			StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
		}

		public Window Window { get; set; }

		public virtual void CloseWindow(bool? result = true)
		{
			Window.DialogResult = result;

			Window?.Close();
		}
	}
}