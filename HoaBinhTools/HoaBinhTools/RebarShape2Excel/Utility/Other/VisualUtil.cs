using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using SingleData;

namespace Utility
{
	public static class VisualUtil
	{

		public static void SetTemporyColorElement(this ElementId elemId)
		{
			OverrideGraphicSettings ogs = new OverrideGraphicSettings();
			Color color = new Color(128, 128, 192);

			FillPatternElement fpe = FillPatternUtil.GetFillPatternElement("<Solid fill>");

			ogs.SetSurfaceForegroundPatternColor(color);
			ogs.SetSurfaceBackgroundPatternColor(color);
			ogs.SetSurfaceForegroundPatternId(fpe.Id);
			ogs.SetSurfaceBackgroundPatternId(fpe.Id);
			ogs.SetSurfaceTransparency(50);

			RevitData.Instance.ActiveView.SetElementOverrides(elemId, ogs);
		}
		public static void SetTemporyColorElement(this List<ElementId> elemIds)
		{
			OverrideGraphicSettings ogs = new OverrideGraphicSettings();
			Color color = new Color(128, 128, 192);

			FillPatternElement fpe = FillPatternUtil.GetFillPatternElement("<Solid fill>");

			ogs.SetSurfaceForegroundPatternColor(color);
			ogs.SetSurfaceBackgroundPatternColor(color);
			ogs.SetSurfaceForegroundPatternId(fpe.Id);
			ogs.SetSurfaceBackgroundPatternId(fpe.Id);
			ogs.SetSurfaceTransparency(50);

			elemIds.ForEach(x => RevitData.Instance.ActiveView.SetElementOverrides(x, ogs));
		}
		public static void RestoreColorElement(this ElementId elemId)
		{
			RevitData.Instance.ActiveView.SetElementOverrides(elemId, new OverrideGraphicSettings());
		}
		public static void RestoreColorElement(this List<ElementId> elemIds)
		{
			elemIds.ForEach(x => RevitData.Instance.ActiveView.SetElementOverrides(x, new OverrideGraphicSettings()));
		}
		public static void SetSectionBox3D(this List<Element> elems)
		{
			var revitData = RevitData.Instance;

			XYZ minZ = XYZ.Zero, maxZ = XYZ.Zero; bool firstAssign = true;
			foreach (Element elem in elems)
			{
				BoundingBoxXYZ bbE = elem.get_BoundingBox(null);
				if (firstAssign)
				{
					firstAssign = false;
					minZ = bbE.Min; maxZ = bbE.Max;
				}
				else
				{
					if (minZ.Z.IsBigger(bbE.Min.Z)) minZ = bbE.Min;
					if (maxZ.Z.IsSmaller(bbE.Max.Z)) maxZ = bbE.Max;
				}
			}

			BoundingBoxXYZ bb = new BoundingBoxXYZ() { Min = minZ - (XYZ.BasisX + XYZ.BasisY + XYZ.BasisZ * 5) * 0.5, Max = maxZ + (XYZ.BasisX + XYZ.BasisY + XYZ.BasisZ * 5) * 0.5 };
			(revitData.ActiveView as View3D).SetSectionBox(bb);
			revitData.Transaction.Commit();
			revitData.Transaction.Start();
			revitData.ActiveUIView.ZoomToFit();
		}
		public static void SetSectionBox3D(this List<ElementId> elemIds)
		{
			SetSectionBox3D(elemIds.Select(x => RevitData.Instance.Document.GetElement(x)).ToList());
		}

		public static void SetTemporaryColorElement(this ElementId elemId)
		{
			OverrideGraphicSettings ogs = new OverrideGraphicSettings();
			Color color = new Color(128, 128, 192);

			FillPatternElement fpe = FillPatternUtil.GetFillPatternElement("<Solid fill>");

			ogs.SetSurfaceForegroundPatternColor(color);
			ogs.SetSurfaceBackgroundPatternColor(color);
			ogs.SetSurfaceForegroundPatternId(fpe.Id);
			ogs.SetSurfaceBackgroundPatternId(fpe.Id);
			ogs.SetSurfaceTransparency(50);

			RevitData.Instance.ActiveView.SetElementOverrides(elemId, ogs);
		}
		public static void SetTemporaryColorElement(this List<ElementId> elemIds)
		{
			OverrideGraphicSettings ogs = new OverrideGraphicSettings();
			Color color = new Color(128, 128, 192);

			FillPatternElement fpe = FillPatternUtil.GetFillPatternElement("<Solid fill>");

			ogs.SetSurfaceForegroundPatternColor(color);
			ogs.SetSurfaceBackgroundPatternColor(color);
			ogs.SetSurfaceForegroundPatternId(fpe.Id);
			ogs.SetSurfaceBackgroundPatternId(fpe.Id);
			ogs.SetSurfaceTransparency(50);

			elemIds.ForEach(x => RevitData.Instance.ActiveView.SetElementOverrides(x, ogs));
		}
	}
}
