using System.Linq;
using SingleData;

namespace Utility
{
	public static partial class FamilyUtil
	{
		private static TemplateData templateData;
		private static RevitData revitData;
		public static Model.Entity.Family AddFamily(string name, string docName,
			Model.Entity.Category category)
		{
			GetSingleData();

			var fam = category.Families.SingleOrDefault(x => x.Name == name);
			if (fam == null)
			{
				fam = new Model.Entity.Family
				{
					Name = name,
					Category = category,
					Project = templateData.Project
				};
				category.Families.Add(fam);
				templateData.Families.Add(fam);
			}
			return fam;
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
