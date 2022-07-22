using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using BeyCons.Core.Libraries.Units;
using BeyCons.Core.RevitUtils.DataUtils;
using HoaBinhTools.BIMQAQC.QAQC_FormWork.ViewModels;
using Utils;

namespace HoaBinhTools.BIMQAQC.QAQC_FormWork.Command.Excutions
{
	public class IntersectElement
	{
        private static List<Element> ElementsOfPart { get; set; }

        public static IList<Element> GetElemenstIntersectWithHostElement(Element elementHost, IList<Element> elementsInSelection)
        {
            BoundingBoxXYZ boundingBoxXYZ = elementHost.get_BoundingBox(null);
            Outline outline = new Outline(boundingBoxXYZ.Min, boundingBoxXYZ.Max);
            BoundingBoxIntersectsFilter boundingBoxIntersectsFilter = new BoundingBoxIntersectsFilter(outline, 5.0.ToFeet());
            IList<Element> elementIntersectsBoundingBox = new FilteredElementCollector(elementHost.Document, elementsInSelection.Select(x => x.Id).ToList()).WherePasses(boundingBoxIntersectsFilter).Excluding(new List<ElementId>() { elementHost.Id }).ToElements();
            if (!(elementHost is Part))
            {
                elementIntersectsBoundingBox = elementIntersectsBoundingBox.Where(x => !(x is Part)).ToList();
            }
            else
            {
                elementIntersectsBoundingBox = FilterElementsWithPart(elementHost as Part, elementIntersectsBoundingBox);
            }
            if (elementIntersectsBoundingBox.Count > 0)
            {
                List<Solid> solidsHost = GeometryUtils.GetSolidsFromInstanceElement(elementHost, new Options() { DetailLevel = ViewDetailLevel.Fine }, true).ToUnionSolids();
                if (solidsHost.Count > 0)
                {
                    List<ElementFilter> elementFilters = new List<ElementFilter>();
                    foreach (Solid solid in solidsHost)
                    {
                        ElementIntersectsSolidFilter elementIntersectsSolidFilter = new ElementIntersectsSolidFilter(solid);
                        elementFilters.Add(elementIntersectsSolidFilter);
                    }
                    LogicalOrFilter logicalOrFilter = new LogicalOrFilter(elementFilters);
                    IList<Element> elementsIntersect = new FilteredElementCollector(elementHost.Document, elementIntersectsBoundingBox.Select(x => x.Id).ToList()).WherePasses(logicalOrFilter).Excluding(new List<ElementId>() { elementHost.Id }).ToElements();
                    List<Element> remainderElementsInFilter = elementIntersectsBoundingBox.Where(p => !elementsIntersect.Any(c => c.Id == p.Id)).ToList();
                    foreach (Element remainderElement in remainderElementsInFilter)
                    {
                        if (ElementUtils.HasIntersectElementWithSolids(remainderElement, solidsHost))
                        {
                            elementsIntersect.Add(remainderElement);
                        }
                    }
                    return elementsIntersect.Count > 0 ? elementsIntersect : null;
                }
            }
            return null;
        }
		public static IList<Element> GetElemenstIntersectWithHostElement(Element elementHost, bool isActiveView)
		{
			BoundingBoxXYZ boundingBoxXYZ = elementHost.get_BoundingBox(null);
			Outline outline = new Outline(boundingBoxXYZ.Min, boundingBoxXYZ.Max);
			BoundingBoxIntersectsFilter boundingBoxIntersectsFilter = new BoundingBoxIntersectsFilter(outline, 5.0.ToFeet());
			IEnumerable<Element> elementIntersectsBoundingBox;
			if (isActiveView)
			{
				elementIntersectsBoundingBox = new FilteredElementCollector(elementHost.Document, ActiveData.ActiveView.Id)
					.WherePasses(boundingBoxIntersectsFilter)
					.Excluding(new List<ElementId>() { elementHost.Id }).ToElements()
					.Where(x => x.Category != null && FormworkViewModels.Categories.Select(c => c.Name).Contains(x.Category.Name));
			}
			else
			{
				elementIntersectsBoundingBox = new FilteredElementCollector(elementHost.Document)
					.WherePasses(boundingBoxIntersectsFilter)
					.Excluding(new List<ElementId>() { elementHost.Id }).ToElements()
					.Where(x => x.Category != null && FormworkViewModels.Categories.Select(c => c.Name).Contains(x.Category.Name));
			}
			List<Element> filterElements = new List<Element>();
			foreach (Element element in elementIntersectsBoundingBox)
			{
				if (FormworkViewModels.CheckInputInformation(element))
				{
					filterElements.Add(element);
				}
			}
			if (filterElements.Count > 0)
			{
				elementIntersectsBoundingBox = filterElements;
				if (!(elementHost is Part))
				{
					elementIntersectsBoundingBox = elementIntersectsBoundingBox.Where(x => !(x is Part)).ToList();
				}
				else
				{
					elementIntersectsBoundingBox = FilterElementsWithPart(elementHost as Part, elementIntersectsBoundingBox);
				}
				if (elementIntersectsBoundingBox.Count() > 0)
				{
					List<Solid> solidsHost = GeometryUtils.GetSolidsFromInstanceElement(elementHost, new Options() { DetailLevel = ViewDetailLevel.Fine }, true).ToUnionSolids();
					if (solidsHost.Count > 0)
					{
						List<ElementFilter> elementFilters = new List<ElementFilter>();
						foreach (Solid solid in solidsHost)
						{
							ElementIntersectsSolidFilter elementIntersectsSolidFilter = new ElementIntersectsSolidFilter(solid);
							elementFilters.Add(elementIntersectsSolidFilter);
						}
						LogicalOrFilter logicalOrFilter = new LogicalOrFilter(elementFilters);
						IList<Element> elementsIntersect = new FilteredElementCollector(elementHost.Document, elementIntersectsBoundingBox.Select(x => x.Id).ToList())
							.WherePasses(logicalOrFilter)
							.Excluding(new List<ElementId>() { elementHost.Id }).ToElements();
						List<Element> remainderElementsInFilter = elementIntersectsBoundingBox.Where(p => !elementsIntersect.Any(c => c.Id == p.Id)).ToList();
						foreach (Element remainderElement in remainderElementsInFilter)
						{
							if (ElementUtils.HasIntersectElementWithSolids(remainderElement, solidsHost))
							{
								elementsIntersect.Add(remainderElement);
							}
						}
						return elementsIntersect.Count > 0 ? elementsIntersect : null;
					}
				}
			}
			return null;
		}
		private static List<Element> FilterElementsWithPart(Part hostPart, IEnumerable<Element> intersectElements)
        {
            List<Element> excludeElements = GetElements(hostPart);
            if (intersectElements.Count() > 0)
            {
                foreach (Element intersectElement in intersectElements)
                {
                    if (intersectElement is Part part)
                    {
                        List<Element> elements = GetElements(part);
                        if (elements.Count > 0)
                        {
                            excludeElements.AddRange(elements);
                        }
                    }
                }
            }
            List<int> ids = excludeElements.Select(x => x.Id.IntegerValue).ToList();
            return intersectElements.Where(x => !ids.Contains(x.Id.IntegerValue)).ToList();
        }
        public static List<Element> GetElements(Part part)
        {
            ElementsOfPart = new List<Element>();
            RecursiveElements(part);
            return ElementsOfPart;
        }
        private static void RecursiveElements(Part part)
        {
            foreach (LinkElementId linkElementId in part.GetSourceElementIds())
            {
                if (null != linkElementId)
                {
                    Element element = part.Document.GetElement(linkElementId.HostElementId);
                    if (null != element)
                    {
                        if (element is Part subPart)
                        {
                            RecursiveElements(subPart);
                        }
                        else
                        {
                            ElementsOfPart.Add(element);
                        }
                    }
                }
            }
        }
        public static IList<Element> GetElemenstIntersectWithSolid(Solid solidFaceHost, IList<Element> intersectElements)
        {
            BoundingBoxXYZ boundingBoxXYZ = solidFaceHost.GetBoundingBox();
            XYZ centerSolidFaceHost = boundingBoxXYZ.Transform.Origin;
            XYZ pointMin = new XYZ(boundingBoxXYZ.Min.X + centerSolidFaceHost.X, boundingBoxXYZ.Min.Y + centerSolidFaceHost.Y, boundingBoxXYZ.Min.Z + centerSolidFaceHost.Z);
            XYZ pointMax = new XYZ(boundingBoxXYZ.Max.X + centerSolidFaceHost.X, boundingBoxXYZ.Max.Y + centerSolidFaceHost.Y, boundingBoxXYZ.Max.Z + centerSolidFaceHost.Z);
            Outline outline = new Outline(pointMin, pointMax);
            BoundingBoxIntersectsFilter boundingBoxIntersectsFilter = new BoundingBoxIntersectsFilter(outline, 5.0.ToFeet());
            IList<Element> elementIntersectsBoundingBox = new FilteredElementCollector(ActiveData.Document, intersectElements.Select(x => x.Id).ToList()).WherePasses(boundingBoxIntersectsFilter).ToElements();
            if (elementIntersectsBoundingBox.Count > 0)
            {
                ElementIntersectsSolidFilter elementIntersectsSolidFilter = new ElementIntersectsSolidFilter(solidFaceHost);
                IList<Element> elementsIntersect = new FilteredElementCollector(ActiveData.Document, elementIntersectsBoundingBox.Select(x => x.Id).ToList()).WherePasses(elementIntersectsSolidFilter).ToElements();
                List<Element> remainderElementsInFilter = elementIntersectsBoundingBox.Where(p => !elementsIntersect.Any(c => c.Id == p.Id)).ToList();
                foreach (Element remainderElement in remainderElementsInFilter)
                {
                    if (ElementUtils.HasIntersectElementWithSolid(remainderElement, solidFaceHost))
                    {
                        elementsIntersect.Add(remainderElement);
                    }
                }
                return elementsIntersect.Count > 0 ? elementsIntersect : null;
            }
            return null;
        }
        public static List<Opening> GetOpeningColumns(Element column)
        {
            BoundingBoxXYZ boundingBoxXYZ = column.get_BoundingBox(null);
            Outline outline = new Outline(boundingBoxXYZ.Min, boundingBoxXYZ.Max);
            BoundingBoxIntersectsFilter boundingBoxIntersectsFilter = new BoundingBoxIntersectsFilter(outline);
            List<Opening> elementIntersectsBoundingBox = new FilteredElementCollector(column.Document).WherePasses(boundingBoxIntersectsFilter).Excluding(new List<ElementId>() { column.Id }).OfCategory(BuiltInCategory.OST_ColumnOpening).Cast<Opening>().Where(x => x.Host.Id == column.Id).ToList();
            return elementIntersectsBoundingBox;
        }
        public static List<Opening> GetOpeningFramings(Element framing)
        {
            BoundingBoxXYZ boundingBoxXYZ = framing.get_BoundingBox(null);
            Outline outline = new Outline(boundingBoxXYZ.Min, boundingBoxXYZ.Max);
            BoundingBoxIntersectsFilter boundingBoxIntersectsFilter = new BoundingBoxIntersectsFilter(outline);
            List<Opening> elementIntersectsBoundingBox = new FilteredElementCollector(framing.Document).WherePasses(boundingBoxIntersectsFilter).Excluding(new List<ElementId>() { framing.Id }).OfCategory(BuiltInCategory.OST_StructuralFramingOpening).Cast<Opening>().Where(x => x.Host.Id == framing.Id).ToList();
            return elementIntersectsBoundingBox;
        }
        public static List<Opening> GetOpeningWalls(Element wall)
        {
            BoundingBoxXYZ boundingBoxXYZ = wall.get_BoundingBox(null);
            Outline outline = new Outline(boundingBoxXYZ.Min, boundingBoxXYZ.Max);
            BoundingBoxIntersectsFilter boundingBoxIntersectsFilter = new BoundingBoxIntersectsFilter(outline);
            List<Opening> elementIntersectsBoundingBox = new FilteredElementCollector(wall.Document).WherePasses(boundingBoxIntersectsFilter).Excluding(new List<ElementId>() { wall.Id }).OfCategory(BuiltInCategory.OST_ArcWallRectOpening).Cast<Opening>().Where(x => x.Host.Id == wall.Id).ToList();
            return elementIntersectsBoundingBox;
        }
        public static bool CanCalculateFormwork(Element element)
        {
            if (element is FamilyInstance familyInstance)
            {
                Family family = familyInstance.Symbol.Family;
                if (family.IsEditable)
                {
                    List<GenericForm> genericForms = GeometryUtils.GetGenericFormsFromFamily(familyInstance, true);
                    if (genericForms.Count > 0)
                    {
                        foreach (GenericForm genericForm in genericForms)
                        {
                            FamilyElementVisibility familyElementVisibility = genericForm.GetVisibility();
                            bool check = familyElementVisibility.IsShownInCoarse && familyElementVisibility.IsShownInMedium && familyElementVisibility.IsShownInFine;
                            if (!check)
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return true;
            }
        }
    }
}
