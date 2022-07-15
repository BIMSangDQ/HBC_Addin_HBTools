using System.Collections.Generic;

namespace Model.Entity
{
	public partial class Navigation
	{
		public int ID { get; set; }
		public Identify Identify { get; set; }
		public List<Element> Elements { get; set; } = new List<Element>();
		public bool IsDelete { get; set; } = false;
	}
}
