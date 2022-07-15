using System.Collections.Generic;
using Utility;

namespace Model.Entity
{
	public partial class Identify : NotifyClass
	{
		public int ID { get; set; }
		public Discipline Discipline { get { return Category.Discipline; } }
		public Category Category { get { return Family.Category; } }
		public Family Family { get { return ElementType.Family; } }
		public ElementType ElementType { get; set; }
		public List<Navigation> Navigations { get; set; } = new List<Navigation>();
		public string Name { get; set; }
		public string Block { get; set; }
		public string Zone { get; set; }
		public Level Level { get; set; }
		public string RoomName { get; set; }
		public string RoomNumber { get; set; }
		public string SystemType { get; set; }
		public string SystemName { get; set; }
		public string ServiceType { get; set; }
		public List<Mass> Masses { get; set; } = new List<Mass>();
		public Project Project { get { return ElementType.Project; } }
		public int Count { get; set; }
		public int CountOnDatabase { get; set; }

		public string FullName
		{
			get
			{
				switch (Discipline.DisciplineType)
				{
					case DisciplineType.Structural:
						return $"{Block}__{Zone}__{Level.Name}__{Name}";
				}
				return Name;
			}
		}
		public string HostName
		{
			get
			{
				switch (Category.BuiltInCategory)
				{
					case Autodesk.Revit.DB.BuiltInCategory.OST_StructuralFraming:
						return $"{Block}__{Zone}__{Level.HostLevel.Name}__{Name}";
					default:
						return FullName;
				}
			}
		}
	}
}
