#region Using
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace BeyCons.Core.RevitUtils.DataUtils
{
    public class DirectShapeUtils
    {
        public static List<Face> FacesError { get; set; }
        public static DirectShape CreateDirectShapeFromGeometryObjects(IList<GeometryObject> geometryObjects, BuiltInCategory builtInCategory)
        {
            DirectShape directShape = DirectShape.CreateElement(RevitData.Instance.Document, new ElementId(builtInCategory));
            if (directShape.IsValidShape(geometryObjects))
            {
                directShape.ApplicationId = $"Application id {Guid.NewGuid()}";
                directShape.ApplicationDataId = $"Geometry object {Guid.NewGuid()}";
                directShape.SetShape(geometryObjects);
                return directShape;
            }
            return null;
        }
        public static List<GeometryObject> GetTessellatedShapeBuilderFromPlanarFaces(List<PlanarFace> planarFaces, Transform transform, ElementId materialId)
        {
            FacesError = new List<Face>();
            try
            {
                TessellatedShapeBuilder tessellatedShapeBuilder = new TessellatedShapeBuilder();
                tessellatedShapeBuilder.OpenConnectedFaceSet(true);
                foreach (Face face in planarFaces)
                {
                    IList<IList<XYZ>> loopVertices = new List<IList<XYZ>>();
                    foreach (EdgeArray edgeArray in face.EdgeLoops)
                    {
                        IList<XYZ> loops = new List<XYZ>();
                        foreach (Edge edge in edgeArray)
                        {
                            Curve curve = edge.AsCurveFollowingFace(face);
                            foreach (XYZ point in curve.Tessellate())
                            {
                                loops.Add(point);
                            }
                        }
                        loopVertices.Add(loops);
                    }
                    TessellatedFace tessellatedFace = new TessellatedFace(loopVertices, materialId);
                    if (tessellatedShapeBuilder.DoesFaceHaveEnoughLoopsAndVertices(tessellatedFace))
                    {
                        tessellatedShapeBuilder.AddFace(tessellatedFace);
                    }
                    else
                    {
                        FacesError.Add(face);
                    }
                }
                tessellatedShapeBuilder.CloseConnectedFaceSet();
                tessellatedShapeBuilder.Target = TessellatedShapeBuilderTarget.AnyGeometry;
                tessellatedShapeBuilder.Fallback = TessellatedShapeBuilderFallback.Mesh;
                tessellatedShapeBuilder.Build();
                TessellatedShapeBuilderResult tessellatedShapeBuilderResult = tessellatedShapeBuilder.GetBuildResult();
                if (tessellatedShapeBuilderResult.Outcome == TessellatedShapeBuilderOutcome.Solid)
                {
                    return tessellatedShapeBuilderResult.GetGeometricalObjects().Select(x => SolidUtils.CreateTransformed(x as Solid, transform)).Cast<GeometryObject>().ToList();
                }
                return tessellatedShapeBuilder.GetBuildResult().GetGeometricalObjects().ToList();
            }
            catch
            {
                TessellatedShapeBuilder tessellatedShapeBuilder = new TessellatedShapeBuilder();
                tessellatedShapeBuilder.OpenConnectedFaceSet(true);
                foreach (Face face in planarFaces)
                {
                    Mesh mesh = face.Triangulate(1);
                    IList<XYZ> outerLoopVertices = new List<XYZ>(3);
                    for (int i = 0; i < mesh.NumTriangles; i++)
                    {
                        MeshTriangle meshTriangle = mesh.get_Triangle(i);
                        outerLoopVertices.Clear();
                        outerLoopVertices.Add(meshTriangle.get_Vertex(0));
                        outerLoopVertices.Add(meshTriangle.get_Vertex(1));
                        outerLoopVertices.Add(meshTriangle.get_Vertex(2));
                        TessellatedFace tessellatedFace = new TessellatedFace(outerLoopVertices, materialId);
                        if (tessellatedShapeBuilder.DoesFaceHaveEnoughLoopsAndVertices(tessellatedFace))
                        {
                            tessellatedShapeBuilder.AddFace(tessellatedFace);
                        }
                        else
                        {
                            FacesError.Add(face);
                        }
                    }
                }
                tessellatedShapeBuilder.CloseConnectedFaceSet();
                tessellatedShapeBuilder.Target = TessellatedShapeBuilderTarget.AnyGeometry;
                tessellatedShapeBuilder.Fallback = TessellatedShapeBuilderFallback.Mesh;
                tessellatedShapeBuilder.Build();
                TessellatedShapeBuilderResult tessellatedShapeBuilderResult = tessellatedShapeBuilder.GetBuildResult();
                if (tessellatedShapeBuilderResult.Outcome == TessellatedShapeBuilderOutcome.Solid)
                {
                    return tessellatedShapeBuilderResult.GetGeometricalObjects().Select(x => SolidUtils.CreateTransformed(x as Solid, transform)).Cast<GeometryObject>().ToList();
                }
                return tessellatedShapeBuilderResult.GetGeometricalObjects().ToList();
            }
        }

        public static List<GeometryObject> GetTessellatedShapeBuilderFromPlanarFaces(List<PlanarFace> planarFaces, ElementId materialId)
        {
            FacesError = new List<Face>();
            try
            {
                TessellatedShapeBuilder tessellatedShapeBuilder = new TessellatedShapeBuilder();
                tessellatedShapeBuilder.OpenConnectedFaceSet(true);
                foreach (Face face in planarFaces)
                {
                    IList<IList<XYZ>> loopVertices = new List<IList<XYZ>>();
                    foreach (EdgeArray edgeArray in face.EdgeLoops)
                    {
                        IList<XYZ> loops = new List<XYZ>();
                        foreach (Edge edge in edgeArray)
                        {
                            Curve curve = edge.AsCurveFollowingFace(face);
                            foreach (XYZ point in curve.Tessellate())
                            {
                                loops.Add(point);
                            }
                        }
                        loopVertices.Add(loops);
                    }
                    TessellatedFace tessellatedFace = new TessellatedFace(loopVertices, materialId);
                    if (tessellatedShapeBuilder.DoesFaceHaveEnoughLoopsAndVertices(tessellatedFace))
                    {
                        tessellatedShapeBuilder.AddFace(tessellatedFace);
                    }
                    else
                    {
                        FacesError.Add(face);
                    }
                }
                tessellatedShapeBuilder.CloseConnectedFaceSet();
                tessellatedShapeBuilder.Target = TessellatedShapeBuilderTarget.AnyGeometry;
                tessellatedShapeBuilder.Fallback = TessellatedShapeBuilderFallback.Mesh;
                tessellatedShapeBuilder.Build();                
                return tessellatedShapeBuilder.GetBuildResult().GetGeometricalObjects().ToList();
            }
            catch
            {
                TessellatedShapeBuilder tessellatedShapeBuilder = new TessellatedShapeBuilder();
                tessellatedShapeBuilder.OpenConnectedFaceSet(true);
                foreach (Face face in planarFaces)
                {
                    Mesh mesh = face.Triangulate(1);
                    IList<XYZ> outerLoopVertices = new List<XYZ>(3);
                    for (int i = 0; i < mesh.NumTriangles; i++)
                    {
                        MeshTriangle meshTriangle = mesh.get_Triangle(i);
                        outerLoopVertices.Clear();
                        outerLoopVertices.Add(meshTriangle.get_Vertex(0));
                        outerLoopVertices.Add(meshTriangle.get_Vertex(1));
                        outerLoopVertices.Add(meshTriangle.get_Vertex(2));
                        TessellatedFace tessellatedFace = new TessellatedFace(outerLoopVertices, materialId);
                        if (tessellatedShapeBuilder.DoesFaceHaveEnoughLoopsAndVertices(tessellatedFace))
                        {
                            tessellatedShapeBuilder.AddFace(tessellatedFace);
                        }
                        else
                        {
                            FacesError.Add(face);
                        }
                    }
                }
                tessellatedShapeBuilder.CloseConnectedFaceSet();
                tessellatedShapeBuilder.Target = TessellatedShapeBuilderTarget.AnyGeometry;
                tessellatedShapeBuilder.Fallback = TessellatedShapeBuilderFallback.Mesh;
                tessellatedShapeBuilder.Build();
                return tessellatedShapeBuilder.GetBuildResult().GetGeometricalObjects().ToList();
            }
        }

        public static List<GeometryObject> GetTessellatedShapeBuilderFromOtherFaces(List<Face> faces, Transform transform, ElementId materialId)
        {
            try
            {
                FacesError = new List<Face>();
                TessellatedShapeBuilder tessellatedShapeBuilder = new TessellatedShapeBuilder();
                tessellatedShapeBuilder.OpenConnectedFaceSet(true);
                foreach (Face face in faces)
                {
                    Mesh mesh = face.Triangulate(1);
                    IList<XYZ> outerLoopVertices = new List<XYZ>(3);
                    for (int i = 0; i < mesh.NumTriangles; i++)
                    {
                        MeshTriangle meshTriangle = mesh.get_Triangle(i);
                        outerLoopVertices.Clear();
                        outerLoopVertices.Add(meshTriangle.get_Vertex(0));
                        outerLoopVertices.Add(meshTriangle.get_Vertex(1));
                        outerLoopVertices.Add(meshTriangle.get_Vertex(2));
                        TessellatedFace tessellatedFace = new TessellatedFace(outerLoopVertices, materialId);
                        if (tessellatedShapeBuilder.DoesFaceHaveEnoughLoopsAndVertices(tessellatedFace))
                        {
                            tessellatedShapeBuilder.AddFace(tessellatedFace);
                        }
                        else
                        {
                            FacesError.Add(face);
                        }
                    }
                }
                tessellatedShapeBuilder.CloseConnectedFaceSet();
                tessellatedShapeBuilder.Target = TessellatedShapeBuilderTarget.AnyGeometry;
                tessellatedShapeBuilder.Fallback = TessellatedShapeBuilderFallback.Mesh;
                tessellatedShapeBuilder.Build();
                TessellatedShapeBuilderResult tessellatedShapeBuilderResult = tessellatedShapeBuilder.GetBuildResult();
                if (tessellatedShapeBuilderResult.Outcome == TessellatedShapeBuilderOutcome.Solid)
                {
                    return tessellatedShapeBuilderResult.GetGeometricalObjects().Select(x => SolidUtils.CreateTransformed(x as Solid, transform)).Cast<GeometryObject>().ToList();
                }
                return tessellatedShapeBuilderResult.GetGeometricalObjects().ToList();
            }
            catch
            {
                return new List<GeometryObject>();
            }
        }

        public static List<GeometryObject> GetTessellatedShapeBuilderFromOtherFaces(List<Face> faces, ElementId materialId)
        {
            try
            {
                FacesError = new List<Face>();
                TessellatedShapeBuilder tessellatedShapeBuilder = new TessellatedShapeBuilder();
                tessellatedShapeBuilder.OpenConnectedFaceSet(true);
                foreach (Face face in faces)
                {
                    Mesh mesh = face.Triangulate(1);
                    IList<XYZ> outerLoopVertices = new List<XYZ>(3);
                    for (int i = 0; i < mesh.NumTriangles; i++)
                    {
                        MeshTriangle meshTriangle = mesh.get_Triangle(i);
                        outerLoopVertices.Clear();
                        outerLoopVertices.Add(meshTriangle.get_Vertex(0));
                        outerLoopVertices.Add(meshTriangle.get_Vertex(1));
                        outerLoopVertices.Add(meshTriangle.get_Vertex(2));
                        TessellatedFace tessellatedFace = new TessellatedFace(outerLoopVertices, materialId);
                        if (tessellatedShapeBuilder.DoesFaceHaveEnoughLoopsAndVertices(tessellatedFace))
                        {
                            tessellatedShapeBuilder.AddFace(tessellatedFace);
                        }
                        else
                        {
                            FacesError.Add(face);
                        }
                    }
                }
                tessellatedShapeBuilder.CloseConnectedFaceSet();
                tessellatedShapeBuilder.Target = TessellatedShapeBuilderTarget.AnyGeometry;
                tessellatedShapeBuilder.Fallback = TessellatedShapeBuilderFallback.Mesh;
                tessellatedShapeBuilder.Build();
                return tessellatedShapeBuilder.GetBuildResult().GetGeometricalObjects().ToList();
            }
            catch
            {
                return new List<GeometryObject>();
            }
        }
    }
}