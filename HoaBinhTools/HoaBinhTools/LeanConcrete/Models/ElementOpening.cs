using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Utils;

namespace HoaBinhTools.LeanConcrete.Models
{
	public static class ElementOpening
	{

		public static int CreateOpening(this Floor Flo, List<CurveArray> ListOpening)
		{
			int count = 0;
			using (Transaction tx = new Transaction(ActiveData.Document))
			{
				tx.Start("Opening");

				if (ListOpening.Count() > 0 && ListOpening != null)
				{
					foreach (var op in ListOpening)
					{
						Flo.Document.Create.NewOpening(Flo, op, true);

						count++;
					}
				}
				tx.Commit();
			}
			return count;
		}
	}
}
