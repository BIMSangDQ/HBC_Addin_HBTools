using System;
using Autodesk.Revit.DB;

namespace HoaBinhTools.FramingInformation.Models
{
	public static class ElementOverridesModels
	{
		public static void ColorElement(Document doc, View view, ElementId id, Byte byt1, Byte byt2, Byte byt3)
		{
			OverrideGraphicSettings ogs = new OverrideGraphicSettings();

			ogs.SetProjectionLineColor(new Color(byt1, byt2, byt3));

			view.SetElementOverrides(id, ogs);
		}

	}
}
