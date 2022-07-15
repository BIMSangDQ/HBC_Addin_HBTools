using System.Collections.Generic;

namespace Model.Entity
{
	public class Category
	{
		public Discipline Discipline { get; set; }
		public string Name { get; set; }
		public int ID { get; set; }
		public Autodesk.Revit.DB.BuiltInCategory BuiltInCategory
		{
			get
			{
				return (Autodesk.Revit.DB.BuiltInCategory)ID;
			}
		}
		public List<Family> Families { get; set; } = new List<Family>();
	}
}
