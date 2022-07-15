using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace Utils
{
	public static class DocumentUtils
	{



		public static FamilySymbol GetFamilySymbol(string symbol, string familyName)
		{
			var sb = new FilteredElementCollector(ActiveData.Document).OfClass(typeof(FamilySymbol)).Cast<FamilySymbol>()
				.FirstOrDefault(x => x.Name == symbol && x.FamilyName == familyName);
			return sb;
		}

		public static BuiltInCategory ToBuiltinCategory(this Category cat)
		{
			BuiltInCategory result = BuiltInCategory.INVALID;
			try
			{
				result = (BuiltInCategory)Enum.Parse(typeof(BuiltInCategory), cat.Id.ToString());
				return result;
			}
			catch
			{
				return result;
			}
		}

		public static Element ToElement(this Reference rf)
		{
			return ActiveData.Document.GetElement(rf);
		}
		public static Element ToElement(this ElementId id)
		{
			return ActiveData.Document.GetElement(id);
		}

		public static List<Element> ToElements(this List<Reference> rfs)
		{
			return rfs.Select(x => x.ToElement()).ToList();
		}
		public static List<T> GetElementsOfType<T>(this Document document, ICollection<ElementId> elementIds) where T : class
		{
			List<T> list = new List<T>();
			foreach (ElementId elementId in elementIds)
			{
				Element element = document.GetElement(elementId);
				if (element is T)
				{
					list.Add(element as T);
				}
			}
			return list;
		}
		public static List<T> GetElements<T>(this Document document) where T : Element
		{
			return new FilteredElementCollector(document).OfClass(typeof(T)).ToElements().Cast<T>().ToList();
		}

		public static List<ElementId> GetElementsIds<T>(this Document document) where T : Element
		{
			return new FilteredElementCollector(document).OfClass(typeof(T)).ToElementIds().ToList();
		}


		public static List<ElementId> GetElementsIds(this Document revitDocument, Type rvtEltType)
		{
			List<ElementId> list = new List<ElementId>();
			FilteredElementCollector filteredElementCollector = new FilteredElementCollector(revitDocument);
			filteredElementCollector.OfClass(rvtEltType);
			FilteredElementIterator elementIterator = filteredElementCollector.GetElementIterator();
			elementIterator.Reset();
			while (elementIterator.MoveNext())
			{
				Element element = elementIterator.Current;
				if (element != null)
				{
					list.Add(element.Id);
				}
			}
			return list;
		}

		public static IList<Element> GetElementsOfCategory(this Document document, BuiltInCategory bic)
		{
			return new FilteredElementCollector(document).OfCategory(bic).ToElements();
		}




		public static IList<Element> GetElementsOfCategory(this Document document, Category bic)
		{
			return new FilteredElementCollector(document).OfCategoryId(bic.Id).ToElements();
		}
		public static IList<Element> FilterElements(this Document document, params Type[] types)
		{
			FilteredElementCollector filteredElementCollector = new FilteredElementCollector(document);
			IList<ElementFilter> list = new List<ElementFilter>();
			foreach (Type type in types)
			{
				list.Add(new ElementClassFilter(type));
			}
			LogicalOrFilter logicalOrFilter = new LogicalOrFilter(list);
			filteredElementCollector.WherePasses(logicalOrFilter);
			return filteredElementCollector.WhereElementIsNotElementType().ToElements();
		}
		public static IList<Element> FilterElements(this Document document, ElementId viewId, List<Type> types)
		{
			FilteredElementCollector filteredElementCollector;
			if (null == viewId)
			{
				filteredElementCollector = new FilteredElementCollector(document);
			}
			else
			{
				filteredElementCollector = new FilteredElementCollector(document, viewId);
			}
			IList<ElementFilter> list = new List<ElementFilter>();
			foreach (Type type in types)
			{
				list.Add(new ElementClassFilter(type));
			}
			LogicalOrFilter logicalOrFilter = new LogicalOrFilter(list);
			filteredElementCollector.WherePasses(logicalOrFilter);
			return filteredElementCollector.WhereElementIsNotElementType().ToElements();
		}

		public static IList<Element> FilterElements(this Document document, params BuiltInCategory[] categories)
		{
			FilteredElementCollector filteredElementCollector = new FilteredElementCollector(document);
			IList<ElementFilter> list = new List<ElementFilter>();
			foreach (BuiltInCategory num in categories)
			{
				BuiltInCategory builtInCategory = num;
				list.Add(new ElementCategoryFilter(builtInCategory));
			}
			LogicalOrFilter logicalOrFilter = new LogicalOrFilter(list);
			filteredElementCollector.WherePasses(logicalOrFilter);
			return filteredElementCollector.WhereElementIsNotElementType().ToElements();
		}
		public static List<ElementId> FilterElementsIds(this Document document, params BuiltInCategory[] categories)
		{
			FilteredElementCollector filteredElementCollector = new FilteredElementCollector(document);
			IList<ElementFilter> list = new List<ElementFilter>();
			foreach (BuiltInCategory num in categories)
			{
				BuiltInCategory builtInCategory = num;
				list.Add(new ElementCategoryFilter(builtInCategory));
			}
			LogicalOrFilter logicalOrFilter = new LogicalOrFilter(list);
			filteredElementCollector.WherePasses(logicalOrFilter);
			return filteredElementCollector.WhereElementIsNotElementType().ToElementIds().ToList<ElementId>();
		}

		public static IList<Element> FilterElements(this Document document, ElementId viewId, params BuiltInCategory[] categories)
		{
			FilteredElementCollector filteredElementCollector;
			if (!(null == viewId))
			{
				filteredElementCollector = new FilteredElementCollector(document, viewId);
			}
			else
			{
				filteredElementCollector = new FilteredElementCollector(document);
			}
			IList<ElementFilter> list = new List<ElementFilter>();
			foreach (BuiltInCategory num in categories)
			{
				BuiltInCategory builtInCategory = num;
				list.Add(new ElementCategoryFilter(builtInCategory));
			}
			LogicalOrFilter logicalOrFilter = new LogicalOrFilter(list);
			filteredElementCollector.WherePasses(logicalOrFilter);
			return filteredElementCollector.WhereElementIsNotElementType().ToElements();
		}

		public static T GetValidElementOrNull<T>(this Document document, ElementId id) where T : Element
		{
			if (id != null)
			{
				T t = document.GetElement(id) as T;
				if (t != null && t.IsValidObject)
				{
					return t;
				}
			}
			return default(T);
		}

		public static List<ElementId> GetElementsFromView(this Document revitDocument, ElementId idView)
		{
			List<ElementId> list = new List<ElementId>();
			FilteredElementCollector filteredElementCollector = new FilteredElementCollector(revitDocument, idView);
			filteredElementCollector.OfClass(typeof(Element));
			FilteredElementIterator elementIterator = filteredElementCollector.GetElementIterator();
			elementIterator.Reset();
			while (elementIterator.MoveNext())
			{
				Element element = elementIterator.Current;
				Element t = element as Element;
				if (t != null)
				{
					list.Add(t.Id);
				}
			}
			return list;
		}


		public static T GetElement<T>(this Document revitDocument, ElementId eltId) where T : Element
		{
			if (ElementId.InvalidElementId == eltId)
			{
				return default;
			}
			Element element = revitDocument.GetElement(eltId);
			return element as T;
		}

		public static List<Material> GetAllRevitMaterials(this Document doc)
		{
			if (doc == null)
			{
				return null;
			}
			FilteredElementCollector filteredElementCollector = new FilteredElementCollector(doc);
			IEnumerable<Material> enumerable = filteredElementCollector.WherePasses(new ElementClassFilter(typeof(Material))).Cast<Material>();
			return enumerable.ToList();
		}
		public static List<RebarShape> GetAllRevitRebarShapes(this Document doc)
		{
			if (doc == null)
			{
				return null;
			}
			FilteredElementCollector filteredElementCollector = new FilteredElementCollector(doc);
			IEnumerable<RebarShape> enumerable = filteredElementCollector.WherePasses(new ElementClassFilter(typeof(RebarShape))).Cast<RebarShape>();

			return enumerable.ToList();
		}

		public static List<Element> IdsToElements(this List<ElementId> ids)
		{
			var eles = new List<Element>();
			foreach (var elementId in ids)
			{
				eles.Add(ActiveData.Document.GetElement(elementId));
			}

			return eles;
		}
		public static List<FamilyInstance> AllFamilyInstanceOfTypeInActiveView(this Document doc, string symbolName)
		{
			var col = new FilteredElementCollector(doc, doc.ActiveView.Id).OfClass(typeof(FamilyInstance)).WhereElementIsNotElementType().Cast<FamilyInstance>();
			return col.Where(x => x.Symbol.Name == symbolName).ToList();
		}

		public static List<Element> GetFromModelByBICs(Document doc, BuiltInCategory[] bics)
		{
			IList<ElementFilter> elementFilterList = new List<ElementFilter>(bics.Count());
			foreach (var bic in bics)
			{
				BuiltInCategory builtInCategory = bic;
				elementFilterList.Add(new ElementCategoryFilter(builtInCategory));
			}
			LogicalOrFilter logicalOrFilter = new LogicalOrFilter(elementFilterList);
			FilteredElementCollector elementCollector = new FilteredElementCollector(doc);
			elementCollector.WherePasses(logicalOrFilter).WhereElementIsNotElementType();
			return elementCollector.ToList();
		}

		public static List<Element> GetFromViewByBICs(this Document doc, View v, BuiltInCategory[] bics)
		{
			IList<ElementFilter> elementFilterList = new List<ElementFilter>(bics.Length);
			foreach (var builtInCategory1 in bics)
			{
				var bic = (int)builtInCategory1;
				var builtInCategory = (BuiltInCategory)bic;
				elementFilterList.Add(new ElementCategoryFilter(builtInCategory));
			}
			var logicalOrFilter = new LogicalOrFilter(elementFilterList);
			var elementCollector = new FilteredElementCollector(doc, v.Id);
			elementCollector.WherePasses(logicalOrFilter).WhereElementIsNotElementType();
			return elementCollector.ToList();
		}
		public static List<Element> GetFromActiveViewByBICs(Document doc, BuiltInCategory[] bics)
		{
			return GetFromViewByBICs(doc, doc.ActiveView, bics);
		}
		public static List<ViewSheet> AllViewSheet(this Document doc)
		{
			return new FilteredElementCollector(doc).OfClass(typeof(ViewSheet)).WhereElementIsNotElementType().ToElements().Cast<ViewSheet>().ToList();
		}

		public static List<ScheduleSheetInstance> AllScheduleSheetInstancesInView(this Document doc, ViewSheet viewSheet)
		{
			return new FilteredElementCollector(doc, viewSheet.Id).OfClass(typeof(ScheduleSheetInstance)).WhereElementIsNotElementType().Cast<ScheduleSheetInstance>().ToList();
		}

	}
}
