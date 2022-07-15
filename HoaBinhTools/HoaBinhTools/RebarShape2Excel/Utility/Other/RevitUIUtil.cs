using System.IO;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using Constant;
using SingleData;

namespace Utility
{
	public static class RevitUIUtil
	{
		public static void AddPushButton
			(this Autodesk.Revit.UI.RibbonPanel panel, string className = "Command")
		{
			PushButtonData pbd = new PushButtonData(ConstantValue.AddinName,
				ConstantValue.AddinName, IOData.Instance.AssemblyFilePath,
				$"{ConstantValue.AddinName}.{className}");

			pbd.LargeImage = BitmapToImageSource(new System.Drawing.Bitmap(
				Path.Combine(IOData.Instance.AssemblyDirectoryPath,
				"Image", $"{ConstantValue.AddinName}.png")));

			panel.AddItem(pbd);
		}
		private static BitmapImage BitmapToImageSource(System.Drawing.Bitmap bitmap)
		{
			using (MemoryStream memory = new MemoryStream())
			{
				bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
				memory.Position = 0;
				BitmapImage bitmapimage = new BitmapImage();
				bitmapimage.BeginInit();
				bitmapimage.StreamSource = memory;
				bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapimage.EndInit();
				return bitmapimage;
			}
		}

	}
}
