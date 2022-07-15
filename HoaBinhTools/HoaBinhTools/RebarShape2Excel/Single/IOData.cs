using System;
using System.IO;
using System.Reflection;

namespace SingleData
{
	public class IOData
	{
		public static IOData Instance { get { return Singleton.Instance.IOData; } }

		#region Variables
		private string assemblyFilePath;
		private string assemblyDirectoryPath;
		private string desktopPath;
		private Assembly entryAssembly;
		#endregion

		#region Properties
		public Assembly EntryAssembly
		{
			get
			{
				if (entryAssembly == null) entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
				return entryAssembly;
			}
		}
		public string AssemblyFilePath
		{
			get
			{
				if (assemblyFilePath == null) assemblyFilePath = Assembly.GetExecutingAssembly().Location;
				return assemblyFilePath;
			}
		}
		public string AssemblyDirectoryPath
		{
			get
			{
				if (assemblyDirectoryPath == null) assemblyDirectoryPath = Path.GetDirectoryName(AssemblyFilePath);
				return assemblyDirectoryPath;
			}
		}
		public string DesktopPath
		{
			get
			{
				if (desktopPath == null) desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
				return desktopPath;
			}
		}
		#endregion
	}
}
