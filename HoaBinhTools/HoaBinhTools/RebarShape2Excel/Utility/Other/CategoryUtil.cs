using System.Collections.Generic;
using System.Linq;
using SingleData;

namespace Utility
{
	public static partial class CategoryUtil
	{
		private static TemplateData templateData;
		private static RevitData revitData;
		public static Model.Entity.Category AddCategory(this Model.Entity.Discipline discipline, int id, string name = null)
		{
			GetSingleData();

			Model.Entity.Category cate = new Model.Entity.Category { ID = id, Discipline = discipline };
			if (name == null)
			{
				Autodesk.Revit.DB.Category revitCate = null;
				try
				{
					revitCate = revitData.Categories.Single(x => x.Id.IntegerValue == id);
				}
				catch
				{
					return null;
				}
				cate.Name = revitCate.Name;
			}
			else
			{
				cate.Name = name;
			}
			templateData.Categories.Add(cate);
			discipline.Categories.Add(cate);
			return cate;
		}
		public static Model.Entity.Category GetCategory(int id, Model.Entity.DisciplineType? disciplineType = null)
		{
			GetSingleData();

			return templateData.Categories.SingleOrDefault(
				x => x.ID == id &&
				(disciplineType == null ||
				(disciplineType != null && x.Discipline.DisciplineType == disciplineType)));
		}
		public static Model.Entity.Category GetValidCategory(this Autodesk.Revit.DB.Element elem)
		{
			GetSingleData();

			var cate = elem.Category;
			if (cate == null) return null;

			var cateId = cate.Id.IntegerValue;
			switch ((Autodesk.Revit.DB.BuiltInCategory)cateId)
			{
				case Autodesk.Revit.DB.BuiltInCategory.OST_Floors:
				case Autodesk.Revit.DB.BuiltInCategory.OST_Walls:
					{
						var projectData = revitData.ProjectInfo;
						if (projectData.BuildingName == "C18003-CH2")
						{
							return GetCategory(cateId, Model.Entity.DisciplineType.Structural);
						}
						var isStructural = elem.LookupParameter("Structural").AsInteger() == 1;
						return GetCategory(cateId, isStructural ? Model.Entity.DisciplineType.Structural : Model.Entity.DisciplineType.Architect);
					}
			}

			return GetCategory(cateId);
		}

		public static Autodesk.Revit.DB.Category GetCategory(this Autodesk.Revit.DB.BuiltInCategory bic)
		{
			GetSingleData();

			return revitData.Categories.Single(x => x.Id.IntegerValue == (int)bic);
		}
		public static Autodesk.Revit.DB.CategorySet GetCategorySet(this List<Autodesk.Revit.DB.BuiltInCategory> bics)
		{
			var cateSet = new Autodesk.Revit.DB.CategorySet();
			foreach (var bic in bics)
			{
				cateSet.Insert(bic.GetCategory());
			}
			return cateSet;
		}
		public static List<Autodesk.Revit.DB.BuiltInCategory> GetBuiltInCategories(this Autodesk.Revit.DB.CategorySet cateSet)
		{
			var bics = new List<Autodesk.Revit.DB.BuiltInCategory>();
			foreach (Autodesk.Revit.DB.Category cate in cateSet)
			{
				bics.Add((Autodesk.Revit.DB.BuiltInCategory)cate.Id.IntegerValue);
			}
			return bics;
		}
		public static bool IsEqual(this List<Autodesk.Revit.DB.BuiltInCategory> bics, List<Autodesk.Revit.DB.BuiltInCategory> otherBics)
		{
			return bics.Except(otherBics).Count() == 0;
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
