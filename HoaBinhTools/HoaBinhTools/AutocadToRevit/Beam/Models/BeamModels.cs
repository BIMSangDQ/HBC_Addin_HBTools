using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using HoaBinhTools.AutocadToRevit.Utils;
using Utils;


namespace HoaBinhTools.AutocadToRevit.Beam.Models
{
	class BeamModels
	{
		public static FamilyInstance Create(BeamData beamData, FamilySymbol familySymbol,
			Level BaseLevel, double BaseOffset, Document doc, string Para, bool IsSetName)
		{
			using (Transaction tx = new Transaction(doc, "CreateFraming"))
			{
				tx.Start();
				Line line = beamData.Centerline;

				var stp = line.GetEndPoint(0);
				var etp = line.GetEndPoint(1);
				double z = BaseLevel.Elevation;

				XYZ p1 = new XYZ(stp.X, stp.Y, z);
				XYZ p2 = new XYZ(etp.X, etp.Y, z);

				DeleteWarningSuper warningSuper = new DeleteWarningSuper();
				FailureHandlingOptions failOpt = tx.GetFailureHandlingOptions();
				failOpt.SetFailuresPreprocessor(warningSuper);
				tx.SetFailureHandlingOptions(failOpt);

				Curve curve = Line.CreateBound(p1, p2) as Curve;
				familySymbol.Activate();

				FamilyInstance instance = doc.Create.NewFamilyInstance(curve, familySymbol, BaseLevel, StructuralType.Beam);

				instance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION)
					.SetValue(BaseOffset.MmToFoot());

				instance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION)
					.Set(BaseOffset.MmToFoot());

				if (IsSetName)
				{

					Parameter ParaName = instance.LookupParameter(Para);

					ParameterUtils.SetValue(ParaName, beamData.TenDam);
				}

				ActiveData.Document.Regenerate();

				//newColumns.Add(instance.Id);
				instance.Pinned = true;

				tx.Commit();
				return instance;
			}
		}
	}
}
