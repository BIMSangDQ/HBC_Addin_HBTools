using Autodesk.Revit.DB;
using Utils;

namespace HoaBinhTools.LeanConcrete.Models
{
	class ElementLeanConcrete
	{
		public static Floor Create(CurveArray CurArry, FloorType FloType, Level Lev, XYZ Normal, string HostName)
		{
			Floor Flo = null;

			using (Transaction tx = new Transaction(ActiveData.Document, "LeanConcrete"))
			{
				tx.Start();

				tx.SetFailuresPreprocessorInTransaction();

				Flo = ActiveData.Document.Create.NewFoundationSlab(CurArry, FloType, Lev, true, Normal);

				Parameter Para = Flo.LookupParameter("ElementName");


				if (Para != null)
				{
					Para.Set(HostName);
				}
				ActiveData.Document.Regenerate();

				tx.Commit();
			}
			return Flo;
		}


	}
}
