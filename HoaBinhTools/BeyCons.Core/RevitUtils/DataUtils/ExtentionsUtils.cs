#region Using
using Autodesk.Revit.DB;
using BeyCons.Core.Libraries.Geometries;
using System.Collections.Generic;
using System.Linq;
using BeyCons.Core.Extensions;
using System;
using System.Collections;
#endregion

namespace BeyCons.Core.RevitUtils.DataUtils
{
    public static class ExtentionUtils
    {
        public static Element ToElement(this Reference reference)
        {
            return RevitData.Instance.Document.GetElement(reference);
        }
        public static Element ToElement(this ElementId elementId)
        {
            return RevitData.Instance.Document.GetElement(elementId);
        }
        public static string ToUserWorkset(this Element element)
        {
            WorksetId worksetId = element.WorksetId;
            if (worksetId != WorksetId.InvalidWorksetId)
            {
                Workset workset = RevitData.Instance.WorksetTable.GetWorkset(worksetId);
                if (workset != null && workset.Owner != "Steven Campbell")
                {
                    return workset.Owner;
                }
            }
            return string.Empty;
        }
        public static List<Curve> Transform(this CurveArray curveArray, Transform transform)
        {
            List<Curve> curvesTransformed = new List<Curve>();
            foreach (Curve curve in curveArray)
            {
                curvesTransformed.Add(curve.CreateTransformed(transform));
            }
            return curvesTransformed;
        }
        public static CurveLoop Clone(this CurveLoop curveLoop)
        {
            if (curveLoop != null)
            {
                CurveLoop curveLoopNew = new CurveLoop();
                foreach (Curve curve in curveLoop)
                {
                    curveLoopNew.Append(curve);
                }
                return curveLoopNew;
            }
            else
            {
                return null;
            }
        }
        public static CurveLoop ToCurveLoop(this CurveArray curveArray)
        {
            CurveLoop curveLoop = new CurveLoop();
            List<Curve> curvesOld = curveArray.ToCurves();

            List<Curve> curvesNew = new List<Curve>() { curvesOld[0] };
            curvesOld.RemoveAt(0);

            while (curvesOld.Count > 0)
            {
                List<Curve> curvesTemp = new List<Curve>();
                for (int i = 0; i < curvesOld.Count; i++)
                {
                    if (GeometryLib.DistanceBetweenTwoPoints(curvesNew.Last().GetEndPoint(1), curvesOld[i].GetEndPoint(0)) < GeometryLib.EPSILON)
                    {
                        curvesNew.Add(curvesOld[i]);
                        curvesTemp.Add(curvesOld[i]);
                    }
                    else if (GeometryLib.DistanceBetweenTwoPoints(curvesNew.Last().GetEndPoint(1), curvesOld[i].GetEndPoint(1)) < GeometryLib.EPSILON)
                    {
                        curvesNew.Add(curvesOld[i].CreateReversed());
                        curvesTemp.Add(curvesOld[i]);
                    }
                }
                curvesOld = curvesOld.Where(x => !curvesTemp.Contains(x)).ToList();
            }
            curvesNew.ForEach(x => curveLoop.Append(x));
            return curveLoop;
        }
        public static List<Curve> ToCurves(this CurveArray curveArray)
        {
            List<Curve> curves = new List<Curve>();
            foreach (Curve curve in curveArray)
            {
                curves.Add(curve);
            }
            return curves;
        }
        public static List<CurveArray> ToCurveArrays(this CurveArray curveArray, Document document)
        {
            List<CurveArray> curveArrays = new List<CurveArray>();
            List<Curve> curves = curveArray.ToCurves();
            while (curves.Count > 0)
            {
                List<Curve> subCurve = new List<Curve>() { curves[0] };
                curves.RemoveAt(0);
                for (int i = 0; i < curves.Count; i++)
                {
                    subCurve[subCurve.Count - 1].Intersect(curves[i], out IntersectionResultArray intersectionResultArray);
                    if (intersectionResultArray != null)
                    {
                        subCurve.Add(curves[i]);
                    }
                    else
                    {
                        break;
                    }
                    intersectionResultArray.Dispose();
                }
                CurveArray subCurveArray = new CurveArray();
                foreach (Curve curve in subCurve)
                {
                    CurveElement curveElement = document.GetElement(curve.Reference) as CurveElement;
                    if (curveElement.LineStyle.Name == "<Sketch>")
                    {
                        subCurveArray.Append(curve);
                    }
                }
                if (subCurveArray.Size > 0)
                {
                    curveArrays.Add(subCurveArray);
                }
                curves = curves.Where(x => !subCurve.Contains(x)).ToList();
            }
            return curveArrays;
        }
        public static CurveArray ToCurveArray(this CurveLoop curveLoop)
        {
            CurveArray curveArray = new CurveArray();
            foreach (Curve curve in curveLoop)
            {
                curveArray.Append(curve);
            }
            return curveArray;
        }
        public static List<Curve> ToCurves(this Face face)
        {
            List<Curve> curves = new List<Curve>();
            foreach (EdgeArray edgeArray in face.EdgeLoops)
            {
                foreach (Edge edge in edgeArray)
                {
                    curves.Add(edge.AsCurveFollowingFace(face));
                }
            }
            return curves;
        }
        public static List<Edge> ToEdges(this Face face)
        {
            List<Edge> curves = new List<Edge>();
            foreach (EdgeArray edgeArray in face.EdgeLoops)
            {
                foreach (Edge edge in edgeArray)
                {
                    curves.Add(edge);
                }
            }
            return curves;
        }
        public static List<string> ToParameterNames(this ParameterMap parameterMap)
        {
            List<string> parameterNames = new List<string>();
            foreach (Parameter parameter in parameterMap)
            {
                parameterNames.Add(parameter.Definition.Name);
            }
            return parameterNames;
        }
        public static List<List<Curve>> ToGroupCurves(this List<Curve> curves)
        {
            List<List<Curve>> groupCurves = new List<List<Curve>>();
            List<Curve> groupCurve = new List<Curve>();

            while (curves.Count > 0)
            {
                if (groupCurve.Count == 0)
                {
                    groupCurve.Add(curves[0]);
                    curves.RemoveAt(0);
                }
                if (curves.Count > 0)
                {
                    int count = 0;
                    List<Curve> subCurves = new List<Curve>();
                    foreach (Curve curve in curves)
                    {
                        if (groupCurve.Last().GetEndPoint(1).DistanceTo(curve.GetEndPoint(0)) < GeometryLib.EPSILON)
                        {
                            groupCurve.Add(curve);
                            subCurves.Add(curve);
                        }
                        else if (groupCurve.Last().GetEndPoint(1).DistanceTo(curve.GetEndPoint(1)) < GeometryLib.EPSILON)
                        {
                            groupCurve.Add(curve.CreateReversed());
                            subCurves.Add(curve);
                        }
                        else if (groupCurve.First().GetEndPoint(0).DistanceTo(curve.GetEndPoint(0)) < GeometryLib.EPSILON)
                        {
                            groupCurve.Insert(0, curve.CreateReversed());
                            subCurves.Add(curve);
                        }
                        else if (groupCurve.First().GetEndPoint(0).DistanceTo(curve.GetEndPoint(1)) < GeometryLib.EPSILON)
                        {
                            groupCurve.Insert(0, curve);
                            subCurves.Add(curve);
                        }
                        else
                        {
                            count++;
                        }
                    }

                    if (count == curves.Count)
                    {
                        groupCurves.Add(new List<Curve>(groupCurve));
                        curves = curves.Except(subCurves).ToList();
                        groupCurve.Clear();
                    }
                    else
                    {
                        curves = curves.Except(subCurves).ToList();
                    }

                    if (curves.Count == 0)
                    {
                        groupCurves.Add(groupCurve);
                    }
                }
                else
                {
                    groupCurves.Add(groupCurve);
                }
            }
            return groupCurves;
        }
        public static CurveArray ToCurveArray(this List<Curve> curves)
        {
            CurveArray curveArray = new CurveArray();
            foreach (Curve curve in curves)
            {
                curveArray.Append(curve);
            }
            return curveArray;
        }
        public static List<Parameter> ToParameters(this ParameterSet parameterSet)
        {
            List<Parameter> parameters = new List<Parameter>();
            foreach (Parameter parameter in parameterSet)
            {
                parameters.Add(parameter);
            }
            return parameters;
        }
        public static List<Parameter> ToParameters(this ParameterMap parameterMap)
        {
            List<Parameter> parameters = new List<Parameter>();
            foreach (Parameter parameter in parameterMap)
            {
                parameters.Add(parameter);
            }
            return parameters;
        }
        public static List<Category> ToCategories(this Categories categories)
        {
            List<Category> cats = new List<Category>();
            foreach (Category category in categories)
            {
                cats.Add(category);
            }
            return cats;
        }
        public static List<Curve> ToCurves(this CurveLoop curveLoop)
        {
            List<Curve> curves = new List<Curve>();
            foreach (Curve curve in curveLoop)
            {
                curves.Add(curve);
            }
            return curves;
        }        
    }
}