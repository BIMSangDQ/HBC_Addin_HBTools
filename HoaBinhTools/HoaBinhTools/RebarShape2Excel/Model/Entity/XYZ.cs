namespace Model.Entity
{
	public partial class XYZ
	{
		public XYZ(Location location, Autodesk.Revit.DB.XYZ revitXYZ)
		{
			Location = location;
			RevitXYZ = revitXYZ;
			X = revitXYZ.X;
			Y = revitXYZ.Y;
			Z = revitXYZ.Z;
		}
		public int ID { get; set; }
		public Location Location { get; set; }
		public Element Element
		{
			get
			{
				return Location.Element;
			}
		}
		public double X { get; set; }
		public double Y { get; set; }
		public double Z { get; set; }
		public Autodesk.Revit.DB.XYZ RevitXYZ { get; set; }
	}
}
