#region Using
using Autodesk.Revit.DB;
using BeyCons.Core.Libraries.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace BeyCons.Core.RevitUtils.DataUtils
{
    public static class IntersectUtils
    {
        public static List<Element> DoIntersect(Element elementIntersect, List<Element> elements, Options options, bool findAbsolute, double offset)
        {
            List<Element> elementsIntersect = new List<Element>();
            BoundingBoxXYZ boundingBoxXYZ = elementIntersect.get_BoundingBox(null);
            Outline outline = new Outline(boundingBoxXYZ.Min, boundingBoxXYZ.Max);
            BoundingBoxIntersectsFilter boundingBoxIntersectsFilter = new BoundingBoxIntersectsFilter(outline, offset);
            List<Element> elementIntersectsBoundingBox = new FilteredElementCollector(elementIntersect.Document,
                elements.Select(x => x.Id).ToList())
                .WherePasses(boundingBoxIntersectsFilter)
                .Excluding(new List<ElementId>() { elementIntersect.Id }).ToList();

            if (findAbsolute)
            {
                List<Solid> solidIntersects = GeometryUtils.GetSolidsFromInstanceElement(elementIntersect, options, true).ToUnionSolids();
                foreach (Element element in elementIntersectsBoundingBox)
                {
                    List<Solid> solidResults = GeometryUtils.GetSolidsFromInstanceElement(element, options, true).ToUnionSolids();
                    foreach (Solid solidResult in solidResults)
                    {
                        int outLoop = 0;
                        foreach (Solid solidIntersect in solidIntersects)
                        {
                            if (DoesIntersect(solidIntersect, solidResult))
                            {
                                elementsIntersect.Add(element);
                                outLoop = 1;
                                break;
                            }
                        }
                        if (outLoop == 1)
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                elementsIntersect = elementIntersectsBoundingBox;
            }
            return elementsIntersect;
        }
        public static bool DoesIntersect(Solid firstSolid, Solid secondSolid)
        {
            if (firstSolid == null || secondSolid == null)
            {
                return false;
            }
            else
            {
                Solid solidOne = SolidUtils.Clone(firstSolid);
                Solid solidTwo = SolidUtils.Clone(secondSolid);
                Solid solidUnion = new List<Solid>() { solidOne, solidTwo }.ToUnionSolid();
                if (null == solidUnion)
                {
                    try
                    {
                        Solid solidIntersect = BooleanOperationsUtils.ExecuteBooleanOperation(solidOne, solidTwo, BooleanOperationsType.Intersect);
                        if (solidIntersect.Volume > GeometryLib.EPSILON_VOLUME)
                        {
                            return true;
                        }
                        else
                        {
                            if (IntersectSolids(solidOne, solidTwo))
                            {
                                return true;
                            }
                            else if (IntersectSolids(solidTwo, solidOne))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    catch
                    {
                        try
                        {
                            Solid solidDifference = BooleanOperationsUtils.ExecuteBooleanOperation(solidOne, solidTwo, BooleanOperationsType.Difference);
                            if (Math.Abs(solidOne.Volume - solidDifference.Volume) > GeometryLib.EPSILON_VOLUME)
                            {
                                return true;
                            }
                            else
                            {
                                if (IntersectSolids(solidOne, solidTwo))
                                {
                                    return true;
                                }
                                else if (IntersectSolids(solidTwo, solidOne))
                                {
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                        catch
                        {
                            if (IntersectSolids(solidOne, solidTwo))
                            {
                                return true;
                            }
                            else if (IntersectSolids(solidTwo, solidOne))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
                else
                {
                    bool booArea = Math.Abs(solidUnion.SurfaceArea - solidOne.SurfaceArea - solidTwo.SurfaceArea) > GeometryLib.EPSILON_VOLUME;
                    bool booEdge = Math.Abs(solidOne.Edges.Size + solidTwo.Edges.Size - solidUnion.Edges.Size) > 0;
                    if (booArea || booEdge)
                    {
                        return true;
                    }
                    else
                    {
                        if (IntersectSolids(solidOne, solidTwo))
                        {
                            return true;
                        }
                        else if (IntersectSolids(solidTwo, solidOne))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
        }
        public static bool IntersectSolids(Solid solidOne, Solid solidTwo)
        {
            foreach (Face faceOne in solidOne.Faces)
            {
                foreach (Edge edge in solidTwo.Edges)
                {
                    IList<XYZ> epts = edge.Tessellate();
                    foreach (XYZ ept in epts)
                    {
                        try
                        {
                            IntersectionResult intersectionResult = faceOne.Project(ept);
                            if (null != intersectionResult)
                            {
                                if (Math.Abs(intersectionResult.Distance) < GeometryLib.EPSILON)
                                {
                                    intersectionResult.Dispose();
                                    return true;
                                }
                                intersectionResult.Dispose();
                            }                            
                        }
                        catch { }
                    }
                }
            }
            return false;
        }
    }
}