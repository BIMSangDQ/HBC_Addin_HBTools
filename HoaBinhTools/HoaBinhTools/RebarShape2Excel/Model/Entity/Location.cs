using System.Collections.Generic;

namespace Model.Entity
{
	public partial class Location
	{
		public int ID
		{
			get
			{
				return Element.ID;
			}
		}
		public Location(Element elem)
		{
			this.Element = elem;
		}
		public Element Element { get; set; }
		public List<XYZ> Points { get; set; }
	}
}
