using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Utility
{
	public class NotifyClass : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		public static event PropertyChangedEventHandler StaticPropertyChanged;

		public static void OnStatisPropertyChanged([CallerMemberName] string propertyName = null)
		{
			StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
		}
	}
}
