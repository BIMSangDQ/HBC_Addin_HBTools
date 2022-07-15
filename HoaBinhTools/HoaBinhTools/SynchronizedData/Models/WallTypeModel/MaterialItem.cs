using Autodesk.Revit.DB;

namespace HoaBinhTools.SynchronizedData.Models
{
	public class MaterialItem
	{
		public string MaterialName { get; set; }
		public ElementId MaterialId { get; set; }

		public MaterialItem(string materialName, ElementId materialId)
		{
			MaterialName = materialName;
			MaterialId = materialId;
		}
	}
}
