using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace HoaBinhTools.SynchronizedData.Converters
{
	public class ConvertColor
	{
		public string Color2String(Color c)
		{
			string Color = c.Red.ToString() + "U+002C" + c.Green.ToString() + "U+002C" + c.Blue.ToString();
			return Color;
		}
		public Color String2Color(string s)
		{
			s = s.Replace("U+002C", "-");
			var v = s.Split('-');
			Color Color = new Color(byte.Parse(v[0]), byte.Parse(v[1]), byte.Parse(v[2]));
			return Color;
		}
	}
}
