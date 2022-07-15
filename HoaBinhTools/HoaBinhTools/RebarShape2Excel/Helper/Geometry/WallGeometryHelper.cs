using Autodesk.Revit.DB;
using SingleData;

namespace Helper
{
	public class WallGeometryHelper : AGeometryHelper
	{
		public WallGeometryHelper(Model.Entity.Geometry geo, Autodesk.Revit.DB.Element elem) : base(geo, elem) { }
		public override void ReadGeometry()
		{
			Curve c = (e.Location as LocationCurve).Curve;
			Line l = c is Line ? c as Line : Line.CreateBound(c.GetEndPoint(0), c.GetEndPoint(1));

			geo.VectorX = l.Direction;
			geo.VectorZ = XYZ.BasisZ;
			geo.VectorY = geo.VectorZ.CrossProduct(geo.VectorX);

			WallType wt = (e as Wall).WallType;

			geo.LengthX = e.LookupParameter("Length").AsDouble();
			geo.LengthY = wt.LookupParameter("Width").AsDouble();

			double z1 = (RevitData.Instance.Document.GetElement(e.LookupParameter("Base Constraint").AsElementId()) as Level).Elevation + e.LookupParameter("Base Offset").AsDouble();
			XYZ p1 = l.GetEndPoint(0);
			geo.Origin = new XYZ(p1.X, p1.Y, z1) - geo.VectorY * geo.LengthY / 2;
			geo.LengthZ = e.LookupParameter("Unconnected Height").AsDouble();

			GetTrueHeight();
			GetSectionPoints();
		}
	}
}
