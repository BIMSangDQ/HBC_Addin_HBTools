using System.Collections.Generic;
using Utility;

namespace Model.Entity
{
	public partial class Element
	{
		public int ID { get; set; }
		public Navigation Navigation { get; set; }
		public Geometry Geometry { get; set; }

		private int elementID = -2;
		public int ElementID
		{
			get
			{
				if (elementID == -2) elementID = RevitElement.Id.IntegerValue;
				return elementID;
			}
		}

		private string guid;
		public string Guid
		{
			get
			{
				if (guid == null) guid = RevitElement.UniqueId;
				return guid;
			}
		}
		public Autodesk.Revit.DB.Element RevitElement { get; set; }

		private Autodesk.Revit.DB.Solid solid;
		public Autodesk.Revit.DB.Solid Solid
		{
			get { if (solid == null) solid = SolidUtil.GetOriginalSolid(RevitElement); return solid; }
		}
		public Category Category { get { return Identify.Category; } }
		public Identify Identify { get { return Navigation.Identify; } }

		private Location location;
		public Location Location
		{
			get
			{
				if (location == null)
				{
					location = this.GetLocation();
				}
				return location;
			}
			set
			{
				location = value;
			}
		}

		private List<Mass> masses;
		public List<Mass> Masses
		{
			get
			{
				if (masses == null)
				{
					masses = this.GetMasses();
				}
				return masses;
			}
		}
	}
}
