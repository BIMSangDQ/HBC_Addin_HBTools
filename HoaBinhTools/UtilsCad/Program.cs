using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Geometry;

namespace UtilsCad
{
	public class Program
	{
		public void draw()
		{
			AutocadConnection acad = new AutocadConnection();
			Point3d p = new Point3d();
			acad.ActiveModelspace.AddCircle(p, 50);
		}
	}
}
