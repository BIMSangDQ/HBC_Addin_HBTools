using System.Collections.Generic;

namespace Model.Entity
{
	public class ElementType
	{
		public int ID { get; set; }
		public Autodesk.Revit.DB.BuiltInCategory BuiltInCategory { get; set; }
		public string Name { get; set; }
		public Family Family { get; set; }
		public Autodesk.Revit.DB.ElementType RevitElementType { get; set; }
		public Category Category { get { return Family.Category; } }
		public Discipline Discipline { get { return Category.Discipline; } }
		public List<Identify> Identifies { get; set; } = new List<Identify>();
		public Project Project { get { return Family.Project; } }
	}
}
