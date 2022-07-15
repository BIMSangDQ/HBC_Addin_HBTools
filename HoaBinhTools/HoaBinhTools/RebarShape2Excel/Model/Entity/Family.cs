using System.Collections.Generic;

namespace Model.Entity
{
	public class Family
	{
		public string Name { get; set; }
		public Category Category { get; set; }
		public List<ElementType> ElementTypes { get; set; } = new List<ElementType>();
		public Project Project { get; set; }
	}
}
