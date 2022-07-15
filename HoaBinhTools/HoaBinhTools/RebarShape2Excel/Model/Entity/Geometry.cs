using System.Collections.Generic;
using Autodesk.Revit.DB;
using Helper;

namespace Model.Entity
{
	public class Geometry
	{
		public Model.Entity.Element Element { get; set; }

		public Autodesk.Revit.DB.XYZ Origin { get; set; }
		public Autodesk.Revit.DB.XYZ VectorX { get; set; }
		public Autodesk.Revit.DB.XYZ VectorY { get; set; }
		public Autodesk.Revit.DB.XYZ VectorZ { get; set; }
		public double LengthX { get; set; }
		public double LengthY { get; set; }
		public double LengthZ { get; set; }
		public double LengthLimit { get; set; }
		public List<Autodesk.Revit.DB.XYZ> SectionPoints { get; set; }
		public double Bottom { get { return Origin.Z; } }
		public double Top { get { return Bottom + LengthZ; } }
		public double TopLimit { get { return Bottom + LengthLimit; } }

		public void ReadGeometry()
		{
			var e = Element.RevitElement;
			if (e is Wall)
			{
				new WallGeometryHelper(this, e).ReadGeometry();
			}
			if (e is FamilyInstance)
			{
				if (e.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns)
				{
					new ColumnGeometryHelper(this, e).ReadGeometry();
				}
				if (e.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
				{
					new BeamGeometryHelper(this, e).ReadGeometry();
				}
			}
		}
	}
}
