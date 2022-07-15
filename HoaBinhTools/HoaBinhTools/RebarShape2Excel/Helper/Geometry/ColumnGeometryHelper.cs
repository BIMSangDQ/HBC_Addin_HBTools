using System;
using Autodesk.Revit.DB;
using SingleData;

namespace Helper
{
	public class ColumnGeometryHelper : AGeometryHelper
	{
		public ColumnGeometryHelper(Model.Entity.Geometry geo, Autodesk.Revit.DB.Element elem) : base(geo, elem) { }
		public override void ReadGeometry()
		{
			Transform tf = (e as FamilyInstance).GetTransform();
			geo.VectorX = tf.BasisX;
			geo.VectorY = tf.BasisY;
			geo.VectorZ = tf.BasisZ;

			Autodesk.Revit.DB.Element et = RevitData.Instance.Document.GetElement(e.GetTypeId());

			try
			{
				geo.LengthX = et.LookupParameter("b").AsDouble();
				geo.LengthY = et.LookupParameter("h").AsDouble();
			}
			catch
			{
				throw new Exception("Your column family Does Not have either 'b' or 'h' Parameter as Dimensions");
			}

			double z1 = (RevitData.Instance.Document.GetElement(e.LookupParameter("Base Level").AsElementId()) as Level).Elevation + e.LookupParameter("Base Offset").AsDouble();
			double z2 = (RevitData.Instance.Document.GetElement(e.LookupParameter("Top Level").AsElementId()) as Level).Elevation + e.LookupParameter("Top Offset").AsDouble();
			geo.Origin = new XYZ(tf.Origin.X, tf.Origin.Y, z1) - geo.VectorX * geo.LengthX / 2 - geo.VectorY * geo.LengthY / 2;
			geo.LengthZ = z2 - z1;

			GetTrueHeight();
			GetSectionPoints();
		}
	}
}
