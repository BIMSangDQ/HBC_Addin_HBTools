using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Utils;

namespace HoaBinhTools.FramingInformation.Models.CreateRebar
{
	public class HorizontalLink
	{
		public void DrawHorizontalLine(Document doc, Element elem, double b, string rebarDia, Line line, XYZ vectorY, double kc, double dk)
		{
			double abv = MathUtils.MmToFoot(double.Parse(Save.Default.ConcreteCover));

			double sp = MathUtils.MmToFoot(kc);

			double len = line.Length;

			int sl = (int)(len / sp) - 1;

			XYZ vectorZ = XYZ.BasisZ;
			XYZ vectorX = vectorY.CrossProduct(vectorZ); ;

			RebarBarType rebarBarType = new FilteredElementCollector(doc)
				.OfClass(typeof(RebarBarType))
				.Cast<RebarBarType>()
				.First(x => x.Name == rebarDia);

			double khoanglui = (len - sl * sp) / 2;

			XYZ pnt = line.GetEndPoint(0) - vectorY * ((MathUtils.MmToFoot(5) + dk)) - vectorZ * (MathUtils.MmToFoot(20)) + vectorX * khoanglui;
			XYZ pnt1 = pnt + vectorY * (b - 2 * abv);

			Line li = Line.CreateBound(pnt, pnt1);

			// Vẽ đai C
			IList<Curve> curves = new List<Curve>();
			curves.Add(li);
			RebarHookType HookStart = (from abcd in new FilteredElementCollector(doc).OfClass(typeof(RebarHookType)).Cast<RebarHookType>()
									   where abcd.Name == Save.Default.Link_HookStart
									   select abcd).First();

			RebarHookType HookEnd = (from abcd in new FilteredElementCollector(doc).OfClass(typeof(RebarHookType)).Cast<RebarHookType>()
									 where abcd.Name == Save.Default.Link_HookEnd
									 select abcd).First();

			RebarHookOrientation ho = RebarHookOrientation.Left;

			RebarHookOrientation hi = RebarHookOrientation.Left;

			Rebar stirrup = Rebar.CreateFromCurves(doc, RebarStyle.StirrupTie, rebarBarType, HookStart, HookEnd, elem, vectorX, curves, ho, hi, true, true);

			RebarShapeDrivenAccessor rebarShapeDrivenAccessor = stirrup.GetShapeDrivenAccessor();
			rebarShapeDrivenAccessor.SetLayoutAsFixedNumber(sl, sp * sl, true, true, true);

		}
	}
}
