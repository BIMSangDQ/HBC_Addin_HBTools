#region Using
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Linq;
using static BeyCons.Core.RevitUtils.DataUtils.Filters.SelectionFilter;
#endregion

namespace BeyCons.Core.RevitUtils.DataUtils
{
    public static class GetObjectUtil
    {
        public static Element PickElement()
        {
            try
            {
                return RevitData.Instance.Document.GetElement(RevitData.Instance.Selection.PickObject(ObjectType.Element, "Pick a element in your project."));
            }
            catch { return null; }
        }
        public static Element PickElement(DelegatesFilter delegatesFilter)
        {
            try
            {
                return RevitData.Instance.Document.GetElement(RevitData.Instance.Selection.PickObject(ObjectType.Element, delegatesFilter, "Pick a element in your project."));
            }
            catch { return null; }
        }

        public static List<Element> PickElements()
        {
            try
            {
                return RevitData.Instance.Selection.PickObjects(ObjectType.Element, GetElementFilter(x => !(x is DirectShape)), "Select elements in your project.").Select(x => RevitData.Instance.Document.GetElement(x)).ToList();
            }
            catch { return null; }
        }
        public static List<Element> PickElements(DelegatesFilter delegatesFilter)
        {
            try
            {
                return RevitData.Instance.Selection.PickObjects(ObjectType.Element, delegatesFilter, "Select elements in your project.").Select(x => RevitData.Instance.Document.GetElement(x)).ToList();
            }
            catch { return null; }
        }

        public static IList<Element> PickElementsByRectangle()
        {
            LogicalOrFilter logicalOrFilter = new LogicalOrFilter(new List<ElementFilter>()
            {
                new ElementClassFilter(typeof(Group), true),
                new ElementClassFilter(typeof(RevitLinkInstance), true),
                new ElementClassFilter(typeof(DirectShape), true)
            });
            return RevitData.Instance.Selection.PickElementsByRectangle(GetElementFilter(logicalOrFilter), "Select elements in your project.");
        }

        public static Face PickFace()
        {
            try
            {
                Reference reference = RevitData.Instance.Selection.PickObject(ObjectType.Face, "Pick a face on element.");
                return RevitData.Instance.Document.GetElement(reference).GetGeometryObjectFromReference(reference) as Face;
            }
            catch { return null; }
        }

        public static List<Face> PickFaces()
        {
            try
            {
                IList<Reference> references = RevitData.Instance.Selection.PickObjects(ObjectType.Face, "Pick the faces on element.");
                return references.Select(x => RevitData.Instance.Document.GetElement(x).GetGeometryObjectFromReference(x) as Face).ToList();
            }
            catch { return null; }
        }

        public static Edge PickEdge()
        {
            try
            {
                Reference reference = RevitData.Instance.Selection.PickObject(ObjectType.Edge, "Pick a face on element.");
                return RevitData.Instance.Document.GetElement(reference).GetGeometryObjectFromReference(reference) as Edge;
            }
            catch { return null; }
        }

        public static List<Reference> PickElementsInRevitLink()
        {
            try
            {
                ElementClassFilter elementClassFilter = new ElementClassFilter(typeof(DirectShape), true);
                return RevitData.Instance.Selection.PickObjects(ObjectType.LinkedElement, GetElementLinkFilter(elementClassFilter), "Select elements in file link.").ToList();
            }
            catch { return null; }
        }

        public static XYZ PickPoint()
        {
            try
            {
                return RevitData.Instance.Selection.PickPoint("Pick a point on work plane.");
            }
            catch { return null; }
        }
    }
}