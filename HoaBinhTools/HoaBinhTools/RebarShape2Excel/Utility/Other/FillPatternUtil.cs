using System.Linq;
using Autodesk.Revit.DB;
using SingleData;

namespace Utility
{
	public static class FillPatternUtil
	{
		private static RevitData revitData;
		public static FillPatternElement GetFillPatternElement(string name)
		{
			GetSingleData();

			return revitData.FillPatternElements.Where(x => x.Name == name).First();
		}
		private static void GetSingleData()
		{
			if (Singleton.Instance != null)
			{
				revitData = RevitData.Instance;
			}
		}
	}
}
