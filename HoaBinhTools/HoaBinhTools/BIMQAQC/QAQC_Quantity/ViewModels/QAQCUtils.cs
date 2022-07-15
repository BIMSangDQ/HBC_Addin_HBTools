using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using BeyCons.Core.Libraries.Geometries;
using BeyCons.Core.RevitUtils.DataUtils;
using HoaBinhTools.BIMQAQC.ModelChecker.ViewModels;
using Utils;

namespace HoaBinhTools.BIMQAQC.QAQC_Quantity.ViewModels
{
	public class QAQCUtils
	{

		public static double GetProximityWidthFraming(Element e)
		{

			ElementId a = e.GetTypeId();
			Element elementType = ActiveData.Document.GetElement(a);

			double b = 0;

			try
			{
				b = elementType.LookupParameter("b").AsDouble();
			}
			catch
			{
				try
				{
					b = elementType.LookupParameter("HB_b").AsDouble();
				}
				catch
				{
					b = elementType.LookupParameter("HB_Width").AsDouble();
				}
			}

			return b;
		}

		public static double GetProximityWidthWall(Element e)
		{

			ElementId a = e.GetTypeId();
			Element elementType = ActiveData.Document.GetElement(a);

			double b = 0;

			try
			{
				b = elementType.LookupParameter("Width").AsDouble();
			}
			catch
			{
				try
				{
					b = elementType.LookupParameter("HB_b").AsDouble();
				}
				catch
				{
					b = elementType.LookupParameter("HB_Width").AsDouble();
				}
			}

			return b;
		}

		public static void InsertFamily(List<XYZ> ListPoint)
		{
			try
			{
				Action action = new Action(() =>
				{
					using (Transaction trans = new Transaction(ActiveData.Document))
					{
						TransactionStatus transactionStatus = trans.Start("Transaction Group");

						var paramId = new ElementId(BuiltInParameter.ALL_MODEL_FAMILY_NAME);
						var paramValueProvider = new ParameterValueProvider(paramId);
						var equalsRule = new FilterStringEquals();
						var filterRule = new FilterStringRule(paramValueProvider, equalsRule, "family1", false);
						var filter = new ElementParameterFilter(filterRule);

						FilteredElementCollector collector = new FilteredElementCollector(ActiveData.Document);
						collector.OfClass(typeof(FamilySymbol)).WhereElementIsElementType().WherePasses(filter);
						FamilySymbol symbol = collector.FirstElement() as FamilySymbol;

						foreach (XYZ point in ListPoint)
						{
							FamilyInstance instance = ActiveData.Document.Create.NewFamilyInstance(point, symbol, StructuralType.NonStructural);
						}

						trans.Commit();
					}
				});
				ExternalEventHandler.Instance.SetAction(action);

				ExternalEventHandler.Instance.Run();
			}
			catch { }
		}

		public static XYZ LocationStartPoint(Element e, double height, double width)
		{
			LocationCurve locCur = e.Location as LocationCurve;
			Curve curve = locCur.Curve;
			Line line = curve as Line;
			XYZ vectorX = line.Direction;
			XYZ vectorZ = XYZ.BasisZ;
			XYZ vectorY = vectorZ.CrossProduct(vectorX);
			XYZ pnt = line.GetEndPoint(0);
			Parameter zJus = e.LookupParameter("z Justification");
			Parameter yJus = e.LookupParameter("y Justification");
			int zPos = zJus.AsInteger();
			double x = 0;
			switch (zPos)
			{
				case 0: // Top   
					x = 0;
					break;
				case 1: // Center  
				case 2: // Origin  
					x = 0.5;
					break;
				case 3: // Bottom  
					x = 1;
					break;
			}

			int yPos = yJus.AsInteger();
			double y = 0;
			switch (yPos)
			{
				case 0: // Left   
					y = 0.5;
					break;
				case 1: //  Center
					break;
				case 2: // Origin 
					break;
				case 3: // Right 
					y = -0.5;
					break;
			}

			XYZ origin = pnt - vectorY * width * y - vectorZ * height * x;
			return origin;
		}

		public static XYZ LocationStartPointWall(Element e, double height, double width)
		{
			LocationCurve locCur = e.Location as LocationCurve;
			Curve curve = locCur.Curve;
			Line line = curve as Line;
			XYZ vectorX = line.Direction;
			XYZ vectorZ = XYZ.BasisZ;
			XYZ vectorY = vectorZ.CrossProduct(vectorX);
			XYZ pnt = line.GetEndPoint(0);
			Parameter locationLinepara = e.get_Parameter(BuiltInParameter.WALL_KEY_REF_PARAM);
			int zPos = locationLinepara.AsInteger();
			double x = 0;
			switch (zPos)
			{
				case 0: // Wall Centerline
					x = 0;
					break;
				case 1: // Core Centerline
					x = 0;
					break;
				case 2: // Finish Face: Exterior
					x = 0.5;
					break;
				case 3: // Finish Face: Interior  
					x = -0.5;
					break;
				case 4: // Core Face: Exterior
					x = 0.5;
					break;
				case 5: // Core Face: Interior  
					x = -0.5;
					break;
			}

			XYZ origin = pnt + vectorY * width * x;// + vectorZ * height;

			//Tìm cao độ đỉnh để gán lại Z
			double ZValue = 0;
			ElementId TopLevelId = e.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).AsElementId();
			if (TopLevelId.IntegerValue != -1)
			{
				Level Toplevel = ActiveData.Document.GetElement(TopLevelId) as Level;
				ZValue = Toplevel.Elevation + e.get_Parameter(BuiltInParameter.WALL_TOP_OFFSET).AsDouble();
			}
			else
			{
				ElementId BaseLevelId = e.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).AsElementId();
				Level BaseLevel = ActiveData.Document.GetElement(BaseLevelId) as Level;
				ZValue = BaseLevel.Elevation + e.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET).AsDouble()
					+ e.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).AsDouble();
			}

			return new XYZ(origin.X, origin.Y, ZValue);
		}

		public static bool IntersectWithCategory(Element element, List<Element> ListElement, Options options, string CategoryNameCheck)
		{
			List<Element> list = IntersectUtils.DoIntersect(element, ListElement, options, true, 0);
			foreach (Element elementcheck in list)
			{
				if (elementcheck.Category.Name == CategoryNameCheck) return true;
			}
			return false;
		}

		public static bool DoesIntersect(Solid solidOne, Solid solidTwo)
		{
			if (solidOne == null || solidTwo == null)
			{
				return false;
			}
			else
			{
				try
				{
					Solid solidUnion = new List<Solid>() { solidOne, solidTwo }.ToUnionSolid();

					bool booArea = Math.Abs(solidUnion.SurfaceArea - solidOne.SurfaceArea - solidTwo.SurfaceArea) > GeometryLib.EPSILON_VOLUME;
					bool booEdge = Math.Abs(solidOne.Edges.Size + solidTwo.Edges.Size - solidUnion.Edges.Size) > 0;
					if (booArea || booEdge)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
				catch
				{
					return false;
				}
			}
		}

		public static List<XYZ> GetCheckPointOfColumn(Element eColumn, Options options)
		{
			List<XYZ> CheckPoints = new List<XYZ>();

			LocationPoint locationPointofColumn = eColumn.Location as LocationPoint;

			double ZValue = 0;
			ElementId TopLevelId = eColumn.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).AsElementId();
			if (TopLevelId.IntegerValue != -1)
			{
				Level Toplevel = ActiveData.Document.GetElement(TopLevelId) as Level;
				ZValue = Toplevel.Elevation + eColumn.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM).AsDouble();
			}
			XYZ locationPoint = new XYZ(locationPointofColumn.Point.X, locationPointofColumn.Point.Y, ZValue);

			List<Solid> solids = GeometryUtils.GetSolidsFromInstanceElement(eColumn, options, true).ToUnionSolids();

			foreach (Solid solid in solids)
			{
				List<PlanarFace> p = GeometryLib.GetPlanarFaceHaveNormalVectorParallelVector(solid, new XYZ(0, 0, 1));
				PlanarFace face = p[0];

				if (face.Area.SquareFootToSquareMeter() <= 0.4)
				{
					if (bhColumn(eColumn) <= 3)
					{
						CheckPoints.Add(locationPoint);
					}
					else
					{
						CheckPoints.Add(locationPoint);
					}
				}
				else
				{
					CheckPoints.Add(locationPoint);
				}
			}

			return CheckPoints;
		}

		public static double bhColumn(Element eColumn)
		{
			try
			{
				Element ColumnType = ActiveData.Document.GetElement(eColumn.GetTypeId());
				double b = 0;
				double h = 0;
				try
				{
					b = ColumnType.LookupParameter("HB_Width").AsDouble();
				}
				catch { }

				try
				{
					h = ColumnType.LookupParameter("HB_Height").AsDouble();
				}
				catch { }

				return h / b;
			}
			catch
			{
				return 1;
			}
		}

		public static bool CheckInterSecElement(List<Element> listElementIntersect, Category c, Element element)
		{
			BoundingBoxXYZ bbe = element.get_BoundingBox(ActiveData.Document.ActiveView);

			double midZ = (bbe.Min.Z + bbe.Max.Z) / 2;

			foreach (Element e in listElementIntersect)
			{
				BoundingBoxXYZ bb = e.get_BoundingBox(ActiveData.Document.ActiveView);
				if (bb.Min.Z > midZ)
				{
					if (e.Category.Name != c.Name)
					{
						if (e.Category.Name == "Structural Framing" ||
							e.Category.Name == "Structural Column" ||
							e.Category.Name == "Floors" ||
							e.Category.Name == "Walls")
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public static double MinElevationZ(List<Element> listElementIntersect, Element element)
		{
			double Minz = 10e10;
			BoundingBoxXYZ bbe = element.get_BoundingBox(ActiveData.Document.ActiveView);

			double midZ = (bbe.Min.Z + bbe.Max.Z) / 2;

			foreach (Element e in listElementIntersect)
			{
				if (e.Category.Name == "Walls" || e.Category.Name == "Structural Column")
				{
					BoundingBoxXYZ bb = e.get_BoundingBox(ActiveData.Document.ActiveView);
					if (bb.Min.Z < Minz && bb.Min.Z > midZ)
					{
						Minz = bb.Min.Z;
					}
				}
			}

			return Minz;
		}

		public static List<Element> ListFloorInTopLevel(List<Element> ListElement, Element element)
		{
			List<Element> resultList = new List<Element>();
			//Get Toplevel
			try
			{
				if (element.Category.Name == "Walls")
				{
					ElementId TopLevelId = element.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).AsElementId();
					if (TopLevelId.IntegerValue != -1)
					{
						resultList = ListElement
							.Where(x => x.get_Parameter(BuiltInParameter.SCHEDULE_LEVEL_PARAM).AsElementId() == TopLevelId).ToList();
					}
				}
			}
			catch { }


			return resultList;
		}
	}
}
