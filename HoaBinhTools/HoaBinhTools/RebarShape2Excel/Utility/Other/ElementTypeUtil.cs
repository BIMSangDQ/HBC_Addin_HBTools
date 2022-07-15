using System.Linq;
using SingleData;

namespace Utility
{
	public static partial class ElementTypeUtil
	{
		private static TemplateData templateData = TemplateData.Instance;
		private static RevitData revitData = RevitData.Instance;
		public static Model.Entity.ElementType AddElementType(Autodesk.Revit.DB.ElementType revitElemType,
			string docName, Model.Entity.Family fam)
		{
			GetSingleData();

			var elemType = fam.ElementTypes.SingleOrDefault(
				x => x.ID == revitElemType.Id.IntegerValue);
			if (elemType == null)
			{
				elemType = new Model.Entity.ElementType
				{
					ID = revitElemType.Id.IntegerValue,
					Name = revitElemType.Name,
					Family = fam,
					RevitElementType = revitElemType,
				};
				fam.ElementTypes.Add(elemType);
				templateData.ElementTypes.Add(elemType);
			}
			return elemType;
		}
		private static void GetSingleData()
		{
			if (Singleton.Instance != null)
			{
				revitData = RevitData.Instance;
				templateData = TemplateData.Instance;
			}
		}
	}
}
