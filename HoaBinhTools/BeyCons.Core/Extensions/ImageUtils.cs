#region Using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
#endregion

namespace BeyCons.Core.Extensions
{
    public class ImageUtils
    {
        public static BitmapSource GetBitmapSource(string pathInProject)
        {
            return BitmapFrame.Create(Assembly.GetExecutingAssembly().GetManifestResourceStream(pathInProject)) as BitmapSource;
        }
    }
}