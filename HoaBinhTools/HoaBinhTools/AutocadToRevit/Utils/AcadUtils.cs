#region Namespaces

using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
#endregion

namespace HoaBinhTools.AutocadToRevit.Utils
{
	public static class AcadUtils
	{
		/// <summary>
		/// Lấy về tất cả Solid có Faces.Count > 0 của file CAD.
		/// </summary>
		/// <param name="cadInstance"></param>
		/// <param name="layerName"></param>
		/// <returns></returns>
		public static List<Solid> GetSolids(ImportInstance cadInstance)
		{
			List<Solid> allSolids = new List<Solid>();

			// Defaults to medium detail, no references and no view.
			Options option = new Options();
			GeometryElement geoElement = cadInstance.get_Geometry(option);

			foreach (GeometryObject geoObject in geoElement)
			{
				if (geoObject is GeometryInstance)
				{
					GeometryInstance geoInstance = geoObject as GeometryInstance;
					GeometryElement geoElement2 = geoInstance.GetInstanceGeometry();
					foreach (GeometryObject geoObject2 in geoElement2)
					{
						if (geoObject2 is Solid)
						{
							Solid solid = geoObject2 as Solid;
							if (solid.SurfaceArea != 0) allSolids.Add(solid);
						}
					}
				}
			}

			return allSolids;
		}

		public static List<PlanarFace> GetHatchHaveName(ImportInstance cadInstance,
			string layerName)
		{
			List<PlanarFace> allHatch = new List<PlanarFace>();

			List<Solid> solids = GetSolids(cadInstance);
			if (solids.Count == 0) return allHatch;

			foreach (Solid solid in solids)
			{
				foreach (Face face in solid.Faces)
				{
					GraphicsStyle graphicsStyle =
						   cadInstance.Document.GetElement(face.GraphicsStyleId)
						   as GraphicsStyle;

					if (graphicsStyle == null) continue;
					Category styleCategory = graphicsStyle.GraphicsStyleCategory;

					if (styleCategory.Name.Equals(layerName))
					{
						allHatch.Add(face as PlanarFace);
					}
				}
			}

			return allHatch;
		}

		public static List<string> GetAllLayer(this ImportInstance cadInstance)
		{
			List<string> allLayers = new List<string>();

			// Defaults to medium detail, no references and no view.
			Options option = new Options();
			option.IncludeNonVisibleObjects = true;
			option.ComputeReferences = true;
			option.DetailLevel = ViewDetailLevel.Fine;
			//option.View = cadInstance.Document.ActiveView;

			GeometryElement geoElement = cadInstance.get_Geometry(option);

			foreach (GeometryObject geoObject in geoElement)
			{
				if (geoObject is GeometryInstance)
				{
					GeometryInstance geoInstance = geoObject as GeometryInstance;
					GeometryElement geoElement2 = geoInstance.GetInstanceGeometry();

					foreach (GeometryObject geoObject2 in geoElement2)
					{
						if (geoObject2 is Solid)
						{
							Solid solid = geoObject2 as Solid;
							//if (solid.Volume < 0.001) continue;

							FaceArray faceArray = solid.Faces;
							foreach (Face face in faceArray)
							{
								ElementId elementId = face.GraphicsStyleId;
								GraphicsStyle graphicsStyle =
									cadInstance.Document.GetElement(elementId)
										as GraphicsStyle;
								Category styleCategory = graphicsStyle.GraphicsStyleCategory;
								allLayers.Add(styleCategory.Name);
							}
						}
						else
						{
							ElementId elementId = geoObject2.GraphicsStyleId;
							GraphicsStyle graphicsStyle =
								cadInstance.Document.GetElement(elementId)
									as GraphicsStyle;
							Category styleCategory = graphicsStyle.GraphicsStyleCategory;
							allLayers.Add(styleCategory.Name);
						}
					}
				}
			}

			allLayers = allLayers.Distinct().ToList();
			allLayers.Sort();
			return allLayers;
		}

		public static List<PolyLine> GetPolylineHaveName(ImportInstance cadInstance,
			string layerName)
		{
			List<PolyLine> allPolylines = new List<PolyLine>();

			Options option = new Options();
			GeometryElement geoElement = cadInstance.get_Geometry(option);

			foreach (GeometryObject geoObject in geoElement)
			{
				if (geoObject is GeometryInstance)
				{
					GeometryInstance geoInstance = geoObject as GeometryInstance;
					GeometryElement geoElement2 = geoInstance.GetInstanceGeometry();

					foreach (GeometryObject geoObject2 in geoElement2)
					{
						if (geoObject2 is PolyLine)
						{
							GraphicsStyle graphicsStyle =
								cadInstance.Document.GetElement(geoObject2.GraphicsStyleId)
									as GraphicsStyle;

							if (graphicsStyle == null) continue;
							Category styleCategory = graphicsStyle.GraphicsStyleCategory;

							if (styleCategory.Name.Equals(layerName))
							{
								allPolylines.Add(geoObject2 as PolyLine);
							}
						}
					}
				}
			}

			return allPolylines;
		}

		public static List<Line> GetLineHaveName(ImportInstance cadInstance)
		{
			List<Line> alllines = new List<Line>();

			Options option = new Options();
			GeometryElement geoElement = cadInstance.get_Geometry(option);

			foreach (GeometryObject geoObject in geoElement)
			{
				if (geoObject is GeometryInstance)
				{
					GeometryInstance geoInstance = geoObject as GeometryInstance;
					GeometryElement geoElement2 = geoInstance.GetInstanceGeometry();

					foreach (GeometryObject geoObject2 in geoElement2)
					{
						if (geoObject2 is Line)
						{
							GraphicsStyle graphicsStyle =
								cadInstance.Document.GetElement(geoObject2.GraphicsStyleId)
									as GraphicsStyle;

							if (graphicsStyle == null) continue;
							Category styleCategory = graphicsStyle.GraphicsStyleCategory;

							if (styleCategory.Name.Contains("BIM_HB~"))
							{
								alllines.Add(geoObject2 as Line);
							}
						}
					}
				}
			}

			return alllines;
		}

		public static List<Line> GetLineHaveNameLayer(ImportInstance cadInstance, string Layer)
		{
			List<Line> alllines = new List<Line>();

			Options option = new Options();
			GeometryElement geoElement = cadInstance.get_Geometry(option);

			foreach (GeometryObject geoObject in geoElement)
			{
				if (geoObject is GeometryInstance)
				{
					GeometryInstance geoInstance = geoObject as GeometryInstance;
					GeometryElement geoElement2 = geoInstance.GetInstanceGeometry();

					foreach (GeometryObject geoObject2 in geoElement2)
					{
						if (geoObject2 is Line)
						{
							GraphicsStyle graphicsStyle =
								cadInstance.Document.GetElement(geoObject2.GraphicsStyleId)
									as GraphicsStyle;

							if (graphicsStyle == null) continue;
							Category styleCategory = graphicsStyle.GraphicsStyleCategory;

							if (styleCategory.Name.Equals(Layer))
							{
								alllines.Add(geoObject2 as Line);
							}
						}
					}
				}
			}

			return alllines;
		}
	}
}
