#region Using
using Autodesk.Revit.DB;
using BeyCons.Core.Libraries.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace BeyCons.Core.RevitUtils.DataUtils
{
    public class ElementUtils
    {
        public static IList<Element> GetIntersectElement(Element elementHost, bool isActiveView)
        {
            BoundingBoxXYZ boundingBoxXYZ = elementHost.get_BoundingBox(null);
            Outline outline = new Outline(boundingBoxXYZ.Min, boundingBoxXYZ.Max);
            BoundingBoxIntersectsFilter boundingBoxIntersectsFilter = new BoundingBoxIntersectsFilter(outline, 5.0.ToFeet());
            IList<Element> elementIntersectsBoundingBox = null;
            if (isActiveView)
            {
                elementIntersectsBoundingBox = new FilteredElementCollector(elementHost.Document, elementHost.Document.ActiveView.Id).WherePasses(boundingBoxIntersectsFilter).Excluding(new List<ElementId>() { elementHost.Id }).ToElements();
            }
            else
            {
                elementIntersectsBoundingBox = new FilteredElementCollector(elementHost.Document).WherePasses(boundingBoxIntersectsFilter).Excluding(new List<ElementId>() { elementHost.Id }).ToElements();
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
                        if (HasIntersectElementWithSolids(remainderElement, solidsHost))
                        {
                            elementsIntersect.Add(remainderElement);
                        }
                    }
                    return elementsIntersect.Count > 0 ? elementsIntersect : null;
                }
            }
            return null;
        }
        public static bool HasIntersectElementWithSolids(Element remainderElement, List<Solid> solidsHost)
        {
            List<Solid> remainderSolids = GeometryUtils.GetSolidsFromInstanceElement(remainderElement, new Options(), true).ToUnionSolids();
            foreach (Solid solidHost in solidsHost)
            {
                foreach (Solid remainderSolid in remainderSolids)
                {
                    if (IntersectUtils.DoesIntersect(solidHost, remainderSolid))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static bool HasIntersectElementWithSolid(Element remainderElement, Solid solidHost)
        {
            List<Solid> remainderSolids = GeometryUtils.GetSolidsFromInstanceElement(remainderElement, new Options(), true).ToUnionSolids();
            foreach (Solid remainderSolid in remainderSolids)
            {
                if (IntersectUtils.DoesIntersect(solidHost, remainderSolid))
                {
                    return true;
                }
            }
            return false;
        }
    }
}