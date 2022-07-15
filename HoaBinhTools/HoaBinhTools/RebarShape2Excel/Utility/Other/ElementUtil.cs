using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using SingleData;

namespace Utility
{
	public static partial class ElementUtil
	{
		private static TemplateData templateData
		{
			get
			{
				return TemplateData.Instance;
			}
		}
		private static RevitData revitData
		{
			get
			{
				return RevitData.Instance;
			}
		}
		private static RevitModelData revitModelData
		{
			get
			{
				return RevitModelData.Instance;
			}
		}
		public static Autodesk.Revit.DB.Element ToRevitElement(this Autodesk.Revit.DB.Reference rf, Document doc = null)
		{
			if (doc == null)
			{
				doc = RevitData.Instance.Document;
			}
			return doc.GetElement(rf);
		}
		public static Autodesk.Revit.DB.Element ToRevitElement(this Autodesk.Revit.DB.ElementId elemId, Document doc = null)
		{
			if (doc == null)
			{
				doc = RevitData.Instance.Document;
			}
			return doc.GetElement(elemId);
		}
		public static Autodesk.Revit.DB.Element ToRevitElement(this int elemId, Document doc = null)
		{
			if (doc == null)
			{
				doc = RevitData.Instance.Document;
			}
			return doc.GetElement(new Autodesk.Revit.DB.ElementId(elemId));
		}
		public static Autodesk.Revit.DB.ElementId ToElementId(this int Id)
		{
			return new Autodesk.Revit.DB.ElementId(Id);
		}

		public static IEnumerable<Model.Entity.Element> GetEntityElements(this IEnumerable<Autodesk.Revit.DB.Element> elems, Func<Model.Entity.Element, bool> filterELem)
		{
			return elems.Select(x => GetEntityElement(x)).Where(filterELem);
		}


		public static void Initial()
		{
			var elements = new List<Model.Entity.Element>();

			var allInstances = revitData.AllInstances
				.Where(x => templateData.Categories.Select(y => (int?)y.BuiltInCategory)
				.Contains(x.Category?.Id.IntegerValue));
			var pgBar = new Model.Control.ProgressBarInstance("Get All Identifies",
				allInstances.Count());
			foreach (var insElem in allInstances)
			{
				pgBar.Step();
				try
				{
					AddEntityElement(insElem);
				}
				catch (Model.Exception.InvalidCategoryException)
				{
					continue;
				}
			}
		}

		public static void AddEntityElement(this Autodesk.Revit.DB.Element insElem)
		{
			var cate = insElem.GetValidCategory();
			if (cate == null) return;
			var discipline = insElem.GetValidDiscipline(cate);

			var doc = insElem.Document;
			var docName = doc.PathName;
			var symbol = insElem.GetTypeId().ToRevitElement(doc) as Autodesk.Revit.DB.ElementType;

			var fam = FamilyUtil.AddFamily(symbol.FamilyName, docName, cate);
			var elemType = ElementTypeUtil.AddElementType(symbol, docName, fam);
			try
			{
				IdentifyUtil.AddIdentify(insElem, docName, elemType);
			}
			catch (Model.Exception.CaseNotCheckException)
			{

			}
			catch (Model.Exception.LevelNotValidException)
			{

			}
		}

		public static void ModifyEntityElement(this Autodesk.Revit.DB.Element insElem)
		{
			DeleteEntityElement(insElem);
			AddEntityElement(insElem);
		}

		public static Model.Entity.Element GetEntityElement
			(this Autodesk.Revit.DB.Element revitElement, Model.Entity.Navigation navigation = null)
		{
			var entElem = templateData.Elements.SingleOrDefault(
				x => x.Guid == revitElement.UniqueId);
			if (entElem == null)
			{
				entElem = new Model.Entity.Element
				{
					RevitElement = revitElement,
					Navigation = navigation,
				};
				templateData.Elements.Add(entElem);
			}
			return entElem;
		}

		public static void DeleteEntityElement(this Autodesk.Revit.DB.Element insElem)
		{
			var entElem = templateData.Elements.SingleOrDefault(x => x.Guid == insElem.UniqueId);
			if (entElem != null)
			{
				var oldNav = entElem.Navigation;
				if (oldNav.Elements.Count == 1)
				{
					var oldIdentify = oldNav.Identify;
					if (oldIdentify.Navigations.Count == 1)
					{
						templateData.Identifies.Remove(oldIdentify);
					}
					else
					{
						oldIdentify.Navigations.Remove(oldNav);
					}
				}
				else
				{
					oldNav.Elements.Remove(entElem);
				}
				templateData.Elements.Remove(entElem);
			}
			else
			{
				revitModelData.DeleteGuids.Add(insElem.UniqueId);
				//AddEntityElement(insElem);

				//var elem = templateData.Elements.Single(x => x.Guid == insElem.UniqueId);
				//elem.Navigation.IsDelete = true;
			}
		}
		public static void DeleteEntityElement2(this Autodesk.Revit.DB.Element insElem)
		{
			var entElem = templateData.Elements.SingleOrDefault(x => x.Guid == insElem.UniqueId);
			if (entElem != null)
			{
				var oldNav = entElem.Navigation;
				if (oldNav.Elements.Count == 1)
				{
					var oldIdentify = oldNav.Identify;
					if (oldIdentify.Navigations.Count == 1)
					{
						templateData.Identifies.Remove(oldIdentify);
					}
					else
					{
						oldIdentify.Navigations.Remove(oldNav);
					}
				}
				else
				{
					oldNav.Elements.Remove(entElem);
				}
				templateData.Elements.Remove(entElem);
			}
			else
			{
				revitModelData.DeleteGuids.Add(insElem.UniqueId);
				AddEntityElement(insElem);

				var elem = templateData.Elements.Single(x => x.Guid == insElem.UniqueId);
				elem.Navigation.IsDelete = true;
			}
		}

		public static Model.Entity.Element GetEntityElement(this int elementID)
		{
			return templateData.Elements.Single(x => x.ElementID == elementID);
		}
	}
}
