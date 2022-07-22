using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using BeyCons.Core.Extensions;
using BeyCons.Core.Libraries.Geometries;
using BeyCons.Core.Libraries.Units;
using BeyCons.Core.RevitUtils.DataUtils;
using HoaBinhTools.BIMQAQC.QAQC_FormWork.Models;
using HoaBinhTools.BIMQAQC.QAQC_FormWork.ViewModels;

namespace HoaBinhTools.BIMQAQC.QAQC_FormWork.Command.Excutions
{
	public class ColumnExcution : VerticalFormwork
	{
		public static ResultFace GetResultFace(FamilyInstance columnElement, IList<Element> intersectElements, ParaUtil paraUtils)
		{
			VerticalFaceFilter verticalFaceFilter = new VerticalFaceFilter() { CommonFaceFilter = new CommonFaceFilter(), FillFaces = new List<Face>() };

			GeometryExcution geometryExcution = new GeometryExcution();
			string explanation = string.Empty;
			int numberOfFaceError = 0;

			Solid columnSolid = GeometryUtils.GetSolidsFromInstanceElement(columnElement, FormworkViewModels.Options, true).ToUnionSolid();
			if (null != columnSolid)
			{
				if (!paraUtils.IsAccurateExplanation)
				{
					if (null != intersectElements)
					{
						if (columnElement.Location is LocationPoint && !columnElement.Symbol.Family.IsInPlace
							&& columnElement.Document.GetElement(columnElement.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).AsElementId()) is Level topLevel)
						{
							//Đáy element giao với cột thấp nhất
							PlanarFace planarFaceBot = GetPlanarFaceBottomOfHeadVerticalElement(topLevel, intersectElements);
							//Mặt element giao với cột thấp nhất
							PlanarFace planarFaceTop = GetPlanarFaceTopOfHeadVerticalElement(topLevel, intersectElements);

							if (null != planarFaceBot)
							{
								try
								{
									List<Solid> solids = new List<Solid>
									{
										BooleanOperationsUtils.CutWithHalfSpace(columnSolid, Plane.CreateByNormalAndOrigin(-XYZ.BasisZ, planarFaceBot.Origin)),
									};

									if (null != planarFaceTop)
									{
										Solid solidfill = BooleanOperationsUtils.CutWithHalfSpace(columnSolid, Plane.CreateByNormalAndOrigin(XYZ.BasisZ, planarFaceBot.Origin));
										try
										{
											List<PlanarFace> planarFacesTopofColumn = GeometryUtils.GetTopPlanarFacesFromInstanceElement(columnElement, FormworkViewModels.Options, true, FormworkViewModels.EpsilonArea);
											PlanarFace planarFacesMaxTopofColumn;

											if (null != planarFacesTopofColumn)
											{
												planarFacesMaxTopofColumn = planarFacesTopofColumn.MaxBy(l => l.Origin.Z);
												//Nếu mặt trên cột cao hơn planarFaceTop => Có 1 đoạn là cp sàn
												if (planarFacesMaxTopofColumn.Origin.Z - planarFaceTop.Origin.Z > FormworkViewModels.EpsilonLenght)
												{
													solids.Add(BooleanOperationsUtils.CutWithHalfSpace(solidfill, Plane.CreateByNormalAndOrigin(-XYZ.BasisZ, planarFaceTop.Origin)));
													solids.Add(BooleanOperationsUtils.CutWithHalfSpace(solidfill, Plane.CreateByNormalAndOrigin(XYZ.BasisZ, planarFaceTop.Origin)));
												}
												else
												{
													solids.Add(solidfill);
												}
											}
										}
										catch
										{
											solids.Add(solidfill);
										}
									}
									verticalFaceFilter = GetFaceFilterVerticalFromSolids(columnElement, geometryExcution, solids, intersectElements, paraUtils, ref numberOfFaceError);
								}
								catch
								{
									verticalFaceFilter = GetFaceFilterVerticalFromSolids(columnElement, geometryExcution, new List<Solid>() { columnSolid }, intersectElements, paraUtils, ref numberOfFaceError);
								}
							}
							else
							{
								verticalFaceFilter = GetFaceFilterVerticalFromSolids(columnElement, geometryExcution, new List<Solid>() { columnSolid }, intersectElements, paraUtils, ref numberOfFaceError);
							}
						}
						else
						{
							verticalFaceFilter = GetFaceFilterVerticalFromSolids(columnElement, geometryExcution, new List<Solid>() { columnSolid }, intersectElements, paraUtils, ref numberOfFaceError);
						}
					}
					else
					{
						verticalFaceFilter.CommonFaceFilter = GetFaceFilterCommonFromVerticalElementSolid(columnElement, geometryExcution, columnSolid, paraUtils);
					}
				}
				else
				{
					verticalFaceFilter.CommonFaceFilter = GetFaceFilterCommonFromVerticalElementSolid(columnElement, geometryExcution, columnSolid, paraUtils);
				}

				if (paraUtils.IsAccurateExplanation)
				{
					explanation = GetFormulaBSide(columnElement, intersectElements, paraUtils);
				}
			}
			else
			{
				//geometryExcution.DataReports.Add(new DataReport(columnElement, null, "Unknow", $"Element has more than one solid") { Index = FormworkViewModels.Index });
			}
			return new ResultFace()
			{
				CommonFaceFilter = verticalFaceFilter.CommonFaceFilter,
				FillFaces = verticalFaceFilter.FillFaces,
				ApproxBSide = explanation,
				//DataReports = geometryExcution.DataReports,
				//NumberOfFaceError = numberOfFaceError,
			};
		}

		//Công thức cho phần cột vách nằm từ đáy giao thấp nhất
		public static string GetFormulaBSide(FamilyInstance columnElement, IList<Element> intersectElements, ParaUtil paraUtil)
		{
			double topElevation = (columnElement.Document.GetElement(columnElement.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).AsElementId()) as Level).Elevation
			+ columnElement.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM).AsDouble();

			double baseElevation = (columnElement.Document.GetElement(columnElement.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM).AsElementId()) as Level).Elevation
				+ columnElement.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM).AsDouble();

			double planarFaceBot = GetPlanarFaceBottomOfHeadVerticalElement(columnElement.Document.GetElement(columnElement.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).AsElementId()) as Level, intersectElements).Origin.Z;

			Solid columnSolid = GeometryUtils.GetSolidsFromInstanceElement(columnElement, FormworkViewModels.Options, true).ToUnionSolid();

			if (null != columnSolid)
			{
				List<Curve> perimeters = new List<Curve>();
				PlanarFace basePlanarFace = GetBottomPlanarFaceVerticalElement(columnSolid);
				if (null != basePlanarFace)
				{
					Solid wallAndColumnSolid = GetSolidWallsAndColumns(columnElement, intersectElements);
					if (null != wallAndColumnSolid)
					{
						perimeters = GetBoundaryVerticalElement(basePlanarFace, wallAndColumnSolid);
					}
					else
					{
						perimeters = basePlanarFace.GetEdgesAsCurveLoops().MaxBy(x => x.GetExactLength()).Select(y => y)
							.Where(z => z.Length > columnElement.Document.Application.ShortCurveTolerance).ToList();
					}

					if (null != perimeters)
					{
						string formula = string.Empty;
						string formula1 = string.Empty;
						string formula2 = string.Empty;

						//Tính mặt cp
						formula = FomularPerimeters(perimeters.Select(x => Math.Round(x.Length.ToMilimeter()).ToString()).ToList(), (planarFaceBot - baseElevation).ToMilimeter());
						return $"={formula}";
					}
				}
			}
			return string.Empty;
		}

		//Công thức cho phần cột vách nằm từ đáy giao thấp nhất
		public static string GetFormulaSide(FamilyInstance columnElement, IList<Element> intersectElements, ParaUtil paraUtil)
		{
			double topElevation = (columnElement.Document.GetElement(columnElement.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).AsElementId()) as Level).Elevation
				+ columnElement.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM).AsDouble();

			double baseElevation = (columnElement.Document.GetElement(columnElement.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM).AsElementId()) as Level).Elevation
				+ columnElement.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM).AsDouble();

			double planarFaceBot = GetPlanarFaceBottomOfHeadVerticalElement(columnElement.Document.GetElement(columnElement.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).AsElementId()) as Level, intersectElements).Origin.Z;

			Solid columnSolid = GeometryUtils.GetSolidsFromInstanceElement(columnElement, FormworkViewModels.Options, true).ToUnionSolid();
			if (null != columnSolid)
			{
				List<Curve> perimeters = new List<Curve>();
				PlanarFace basePlanarFace = GetBottomPlanarFaceVerticalElement(columnSolid);
				if (null != basePlanarFace)
				{
					Solid wallAndColumnSolid = GetSolidWallsAndColumns(columnElement, intersectElements);
					if (null != wallAndColumnSolid)
					{
						perimeters = GetBoundaryVerticalElement(basePlanarFace, wallAndColumnSolid);
					}
					else
					{
						perimeters = basePlanarFace.GetEdgesAsCurveLoops().MaxBy(x => x.GetExactLength()).Select(y => y).Where(z => z.Length > columnElement.Document.Application.ShortCurveTolerance).ToList();
					}

					if (null != perimeters)
					{
						string formula = string.Empty;
						//if (paraUtil.IsAccurateExplanation)
						//{
						//	formula += $"({string.Join("+", perimeters.Select(x => Math.Round(x.Length.ToMilimeter()).ToString()).ToArray())})*{Math.Round((topElevation - baseElevation).ToMilimeter())}";
						//	formula += GetFormulaColumn(columnElement, intersectElements, perimeters, topElevation, baseElevation, planarFaceBot);
						//}
						//else
						//{
						//	formula += GetFormulaColumn(columnElement, intersectElements, perimeters, topElevation, baseElevation, planarFaceBot);
						//}
						return $"={formula}";
					}
				}
			}
			return string.Empty;
		}
		//private static string GetFormulaColumn(FamilyInstance column, IList<Element> intersectElements, List<Curve> perimeters, double topElevation, double baseElevation, double planarFaceBot)
		//{
		//	string formula = string.Empty;
		//	if (null != intersectElements)
		//	{
		//		List<Floor> floors = GetFloors(column, intersectElements);
		//		if (null != floors)
		//		{
		//			if (CheckSubtractFormulaFloors(column, floors))
		//			{
		//				formula += $"({string.Join("+", perimeters.Select(x => Math.Round(x.Length.ToMilimeter()).ToString()).ToArray())})*{Math.Round((topElevation - baseElevation).ToMilimeter())}";
		//				Solid solidPerimeters = CreateExtrusionFromCurves(perimeters, topElevation - baseElevation);
		//				if (null != solidPerimeters)
		//				{
		//					foreach (Floor floor in floors)
		//					{
		//						double thicknessFloorFeet = floor.get_Parameter(BuiltInParameter.FLOOR_ATTR_THICKNESS_PARAM).AsDouble();
		//						Solid floorSolid = GeometryUtils.GetSolidsFromInstanceElement(floor, FormworkViewModels.Options, true).ToUnionSolid();
		//						if (null != floorSolid)
		//						{
		//							try
		//							{
		//								Solid intersectSolid = BooleanOperationsUtils.ExecuteBooleanOperation(solidPerimeters, floorSolid, BooleanOperationsType.Intersect);
		//								if (intersectSolid.Volume > FormworkViewModels.EpsilonVolume)
		//								{
		//									formula += $"-{Math.Round((intersectSolid.Volume / (FormworkViewModels.Thickness / 2) / thicknessFloorFeet).ToMilimeter())}*{Math.Round(thicknessFloorFeet.ToMilimeter())}";
		//								}
		//							}
		//							catch { }
		//						}
		//					}
		//				}
		//				List<FamilyInstance> framings = GetStructuralFramings(column, intersectElements);
		//				if (null != framings)
		//				{
		//					double thicknessFloorsFeet = floors.Select(x => x.get_Parameter(BuiltInParameter.FLOOR_ATTR_THICKNESS_PARAM).AsDouble()).Average();
		//					foreach (FamilyInstance framing in framings)
		//					{
		//						List<Line> widthAndHeight = InforElementUtils.GetWidthHeightFraming(framing, FormworkViewModels.Options);
		//						double heightFramingFeet = null != widthAndHeight ? widthAndHeight[1].Length : 0;
		//						if (heightFramingFeet != 0)
		//						{
		//							Solid framingSolid = GeometryUtils.GetSolidsFromInstanceElement(framing, FormworkViewModels.Options, true).ToUnionSolid();
		//							if (null != framingSolid)
		//							{
		//								try
		//								{
		//									Solid intersectSolid = BooleanOperationsUtils.ExecuteBooleanOperation(solidPerimeters, framingSolid, BooleanOperationsType.Intersect);
		//									if (intersectSolid.Volume > FormworkViewModels.EpsilonVolume)
		//									{
		//										if (IsOriginFramingIntersectFloor(framing, floors))
		//										{
		//											PlanarFace topPlanarFaceFraming = GeometryUtils.GetTopPlanarFacesFromSolid(framingSolid, GeometryLib.EPSILON_AREA).MaxBy(x => x.Origin.Z);
		//											if (topPlanarFaceFraming.Origin.Z > topElevation)
		//											{
		//												double elevation = topPlanarFaceFraming.Origin.Z - topElevation;
		//												formula += $"-{Math.Round((intersectSolid.Volume / (FormworkViewModels.Thickness / 2) / (heightFramingFeet - thicknessFloorsFeet - elevation)).ToMilimeter())}*{Math.Round((heightFramingFeet - thicknessFloorsFeet - elevation).ToMilimeter())}";
		//											}
		//											else
		//											{
		//												formula += $"-{Math.Round((intersectSolid.Volume / (FormworkViewModels.Thickness / 2) / (heightFramingFeet - thicknessFloorsFeet)).ToMilimeter())}*{Math.Round((heightFramingFeet - thicknessFloorsFeet).ToMilimeter())}";
		//											}
		//										}
		//										else
		//										{
		//											PlanarFace topPlanarFaceFraming = GeometryUtils.GetTopPlanarFacesFromSolid(framingSolid, GeometryLib.EPSILON_AREA).MaxBy(x => x.Origin.Z);
		//											if (topPlanarFaceFraming.Origin.Z > topElevation)
		//											{
		//												double elevation = topPlanarFaceFraming.Origin.Z - topElevation;
		//												formula += $"-{Math.Round((intersectSolid.Volume / (FormworkViewModels.Thickness / 2) / (heightFramingFeet - elevation)).ToMilimeter())}*{Math.Round((heightFramingFeet - elevation).ToMilimeter())}";
		//											}
		//											else
		//											{
		//												formula += $"-{Math.Round((intersectSolid.Volume / (FormworkViewModels.Thickness / 2) / heightFramingFeet).ToMilimeter())}*{Math.Round(heightFramingFeet.ToMilimeter())}";
		//											}
		//										}
		//									}
		//								}
		//								catch { }
		//							}
		//						}
		//					}
		//				}
		//				solidPerimeters?.Dispose();
		//			}
		//			else
		//			{
		//				PlanarFace minBottomPlanarFaceFloor = floors.Select(x => GeometryUtils.GetBottomPlanarFacesFromInstanceElement(x, FormworkViewModels.Options, true, FormworkViewModels.EpsilonArea).MinBy(y => y.Origin.Z)).MinBy(z => z.Origin.Z);
		//				formula += $"({string.Join("+", perimeters.Select(x => Math.Round(x.Length.ToMilimeter()).ToString()).ToArray())})*{Math.Round((minBottomPlanarFaceFloor.Origin.Z - baseElevation).ToMilimeter())}";
		//				Solid solidPerimeters = CreateExtrusionFromCurves(perimeters, minBottomPlanarFaceFloor.Origin.Z - baseElevation);
		//				if (null != solidPerimeters)
		//				{
		//					List<FamilyInstance> framings = GetStructuralFramings(column, intersectElements);
		//					if (null != framings)
		//					{
		//						double thicknessFloorsFeet = floors.Select(x => x.get_Parameter(BuiltInParameter.FLOOR_ATTR_THICKNESS_PARAM).AsDouble()).Average();
		//						foreach (FamilyInstance framing in framings)
		//						{
		//							List<Line> widthAndHeight = InforElementUtils.GetWidthHeightFraming(framing, FormworkViewModels.Options);
		//							double heightFramingFeet = null != widthAndHeight ? widthAndHeight[1].Length : 0;
		//							if (heightFramingFeet != 0)
		//							{
		//								Solid framingSolid = GeometryUtils.GetSolidsFromInstanceElement(framing, FormworkViewModels.Options, true).ToUnionSolid();
		//								if (null != framingSolid)
		//								{
		//									try
		//									{
		//										Solid intersectSolid = BooleanOperationsUtils.ExecuteBooleanOperation(solidPerimeters, framingSolid, BooleanOperationsType.Intersect);
		//										if (intersectSolid.Volume > FormworkViewModels.EpsilonVolume)
		//										{
		//											if (IsFramingIntersectFloor(framingSolid, floors))
		//											{
		//												PlanarFace topPlanarFaceFraming = GeometryUtils.GetTopPlanarFacesFromSolid(framingSolid, GeometryLib.EPSILON_AREA).MaxBy(x => x.Origin.Z);
		//												if (topPlanarFaceFraming.Origin.Z > topElevation)
		//												{
		//													double elevation = topPlanarFaceFraming.Origin.Z - topElevation;
		//													formula += $"-{Math.Round((intersectSolid.Volume / (FormworkViewModels.Thickness / 2) / (heightFramingFeet - thicknessFloorsFeet - elevation)).ToMilimeter())}*{Math.Round((heightFramingFeet - thicknessFloorsFeet - elevation).ToMilimeter())}";
		//												}
		//												else
		//												{
		//													formula += $"-{Math.Round((intersectSolid.Volume / (FormworkViewModels.Thickness / 2) / (heightFramingFeet - thicknessFloorsFeet)).ToMilimeter())}*{Math.Round((heightFramingFeet - thicknessFloorsFeet).ToMilimeter())}";
		//												}
		//											}
		//											else
		//											{
		//												PlanarFace topPlanarFaceFraming = GeometryUtils.GetTopPlanarFacesFromSolid(framingSolid, GeometryLib.EPSILON_AREA).MaxBy(x => x.Origin.Z);
		//												if (topPlanarFaceFraming.Origin.Z > topElevation)
		//												{
		//													double elevation = topPlanarFaceFraming.Origin.Z - topElevation;
		//													formula += $"-{Math.Round((intersectSolid.Volume / (FormworkViewModels.Thickness / 2) / (heightFramingFeet - elevation)).ToMilimeter())}*{Math.Round((heightFramingFeet - elevation).ToMilimeter())}";
		//												}
		//												else
		//												{
		//													formula += $"-{Math.Round((intersectSolid.Volume / (FormworkViewModels.Thickness / 2) / heightFramingFeet).ToMilimeter())}*{Math.Round(heightFramingFeet.ToMilimeter())}";
		//												}
		//											}
		//										}
		//									}
		//									catch { }
		//								}
		//							}
		//						}
		//					}
		//				}
		//				solidPerimeters?.Dispose();
		//			}
		//		}
		//		else
		//		{
		//			formula += $"({string.Join("+", perimeters.Select(x => Math.Round(x.Length.ToMilimeter()).ToString()).ToArray())})*{Math.Round((topElevation - baseElevation).ToMilimeter())}";
		//			Solid solidPerimeters = CreateExtrusionFromCurves(perimeters, topElevation - baseElevation);
		//			if (null != solidPerimeters)
		//			{
		//				List<FamilyInstance> framings = GetStructuralFramings(column, intersectElements);
		//				if (null != framings)
		//				{
		//					foreach (FamilyInstance framing in framings)
		//					{
		//						if (CheckSubtractFormulaFramings(column, framings))
		//						{
		//							List<Line> widthAndHeight = InforElementUtils.GetWidthHeightFraming(framing, FormworkViewModels.Options);
		//							double heightFramingFeet = null != widthAndHeight ? widthAndHeight[1].Length : 0;
		//							if (heightFramingFeet != 0)
		//							{
		//								Solid framingSolid = GeometryUtils.GetSolidsFromInstanceElement(framing, FormworkViewModels.Options, true).ToUnionSolid();
		//								if (null != framingSolid)
		//								{
		//									try
		//									{
		//										Solid intersectSolid = BooleanOperationsUtils.ExecuteBooleanOperation(solidPerimeters, framingSolid, BooleanOperationsType.Intersect);
		//										if (intersectSolid.Volume > FormworkViewModels.EpsilonVolume)
		//										{
		//											formula += $"-{Math.Round((intersectSolid.Volume / (FormworkViewModels.Thickness / 2) / heightFramingFeet).ToMilimeter())}*{Math.Round(heightFramingFeet.ToMilimeter())}";
		//										}
		//									}
		//									catch { }
		//								}
		//							}
		//						}
		//					}
		//				}
		//			}
		//			solidPerimeters?.Dispose();
		//		}
		//	}
		//	else
		//	{
		//		formula += $"({string.Join("+", perimeters.Select(x => Math.Round(x.Length.ToMilimeter()).ToString()).ToArray())})*{Math.Round((topElevation - baseElevation).ToMilimeter())}";
		//	}
		//	return formula;
		//}
		private static List<FamilyInstance> GetStructuralFramings(FamilyInstance columnElement, IList<Element> intersectElements)
		{
			if (null != intersectElements)
			{
				IEnumerable<FamilyInstance> familyInstances = intersectElements.Where(x => x is FamilyInstance familyInstance && familyInstance.Category.Name == FormworkViewModels.Categories[2].Name && !familyInstance.Symbol.Family.IsInPlace && x.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM).AsElementId() == columnElement.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).AsElementId() && x.get_Parameter(BuiltInParameter.STRUCTURAL_BEND_DIR_ANGLE).AsDouble() < GeometryLib.EPSILON && Math.Abs(x.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION).AsDouble() - x.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION).AsDouble()) < GeometryLib.EPSILON).Cast<FamilyInstance>();
				if (familyInstances.Count() > 0)
				{
					return familyInstances.ToList();
				}
			}
			return null;
		}
		private static List<Floor> GetFloors(FamilyInstance columnElement, IList<Element> intersectElements)
		{
			if (null != intersectElements)
			{
				IEnumerable<Floor> floors = intersectElements.Where(x => x is Floor floor && floor.get_Parameter(BuiltInParameter.LEVEL_PARAM).AsElementId() == columnElement.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).AsElementId()).Cast<Floor>().Where(x => !x.SlabShapeEditor.IsEnabled);
				if (floors.Count() > 0)
				{
					return floors.ToList();
				}
			}
			return null;
		}
		private static Solid GetSolidWallsAndColumns(FamilyInstance columnElement, IList<Element> intersectElements)
		{
			if (null != intersectElements)
			{
				IEnumerable<Element> wallsAndColumns = intersectElements.Where(x => (x is Wall wall
				&& wall.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).AsElementId() == columnElement.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM).AsElementId())
				|| (x is FamilyInstance familyInstance
				&& familyInstance.Category.Name == FormworkViewModels.Categories[0].Name && !familyInstance.Symbol.Family.IsInPlace
				&& familyInstance.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM).AsElementId() == columnElement.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM)
				.AsElementId())).Where(y => y.Id != columnElement.Id);

				if (wallsAndColumns.Count() > 0)
				{
					Solid wallSolid = wallsAndColumns.Select(x => GeometryUtils.GetSolidsFromInstanceElement(x, FormworkViewModels.Options, true)).SelectMany(y => y).ToList().ToUnionSolid();
					if (null != wallSolid)
					{
						return wallSolid;
					}
				}
			}
			return null;
		}

	}
}
