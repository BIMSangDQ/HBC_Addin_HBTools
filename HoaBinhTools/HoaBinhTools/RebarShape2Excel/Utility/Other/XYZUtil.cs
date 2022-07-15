using System.Linq;
using SingleData;

namespace Utility
{
	public static partial class XYZUtil
	{
		private static TemplateData templateData
		{
			get
			{
				return TemplateData.Instance;
			}
		}
		public static Model.Entity.XYZ GetPoint(this Autodesk.Revit.DB.XYZ revitPnt,
			Model.Entity.Location location)
		{
			var pnt = location.Points.SingleOrDefault(x => x.RevitXYZ.IsEqual(revitPnt));
			if (pnt == null)
			{
				pnt = new Model.Entity.XYZ(location, revitPnt);

				location.Points.Add(pnt);
				templateData.XYZs.Add(pnt);
			}
			return pnt;
		}
	}
}
