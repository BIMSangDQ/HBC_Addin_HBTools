using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using SingleData;

namespace Utility
{
	public static partial class RebarUtil
	{
		private static RevitData revitData
		{
			get
			{
				return RevitData.Instance;
			}
		}
		private static RevitModelData revitModelData
		{
			get
			{
				return RevitModelData.Instance;
			}
		}
		public static Model.Entity.Mass GetRebarMass(this Model.Entity.Element entElem)
		{
			var mass = entElem.GetMass(Model.Entity.MassType.Rebar);
			double value = 0;

			var hostName = entElem.Identify.HostName;
			IEnumerable<Autodesk.Revit.DB.Structure.Rebar> rebars = revitData.Rebars.Where(x => x.LookupParameter("HB_ElementName").AsString() == hostName);
			value = rebars.Sum(x =>
			{
				var rebarType = x.GetTypeId().ToRevitElement() as Autodesk.Revit.DB.Structure.RebarBarType;
				var valueFeet = x.get_Parameter(BuiltInParameter.REBAR_ELEM_TOTAL_LENGTH).AsDouble();
				var valueKgPerFeet = rebarType.LookupParameter("Trong luong/met dai (kg/m)").AsDouble();
				return valueFeet * valueKgPerFeet;
			});

			mass.WeightMassValue = new Model.Entity.MassValue
			{
				Name = "Rebar",
				Value = value
			};

			return mass;
		}

		public static Autodesk.Revit.DB.Structure.RebarHookOrientation GetHookOrient
			(Autodesk.Revit.DB.XYZ curveVec, Autodesk.Revit.DB.XYZ normal,
			Autodesk.Revit.DB.XYZ hookVec)
		{
			Autodesk.Revit.DB.XYZ tempVec = normal;

			for (int i = 0; i < 4; i++)
			{
				tempVec = tempVec.CrossProduct(curveVec);
				if (tempVec.IsSameDirection(hookVec))
				{
					if (i == 0)
					{
						return Autodesk.Revit.DB.Structure.RebarHookOrientation.Right;
					}
					else if (i == 2)
					{
						return Autodesk.Revit.DB.Structure.RebarHookOrientation.Left;
					}
				}
			}

			throw new Exception("Can't find the hook orient according to hook direction.");
		}

		public static Autodesk.Revit.DB.Structure.RebarShapeDrivenAccessor GetRebarShapeDrivenAccessor
			(this Model.Entity.Rebar rebar)
		{
			if (rebar.SingleRebar != null)
			{
				return rebar.SingleRebar.GetShapeDrivenAccessor();
			}
			return null;
		}

		public static Autodesk.Revit.DB.Structure.RebarShape GetRebarShape(this Model.Entity.Rebar rebar)
		{
			if (rebar.SingleRebar != null)
			{
				return rebar.SingleRebar.GetShapeId().ToRevitElement() as Autodesk.Revit.DB.Structure.RebarShape;
			}
			else
			{
				return rebar.SystemRebar.RebarShapeId.ToRevitElement()
					as Autodesk.Revit.DB.Structure.RebarShape;
			}
		}

		public static Autodesk.Revit.DB.Structure.RebarBarType GetRebarBarType
			(this Model.Entity.Rebar rebar)
		{
			if (rebar.SingleRebar != null)
			{
				return rebar.SingleRebar.GetTypeId().ToRevitElement()
					as Autodesk.Revit.DB.Structure.RebarBarType;
			}
			else
			{
				return rebar.SystemRebar.GetTypeId().ToRevitElement()
					as Autodesk.Revit.DB.Structure.RebarBarType;
			}
		}

		public static Autodesk.Revit.DB.Structure.RebarBendData GetRebarBendData
			(this Model.Entity.Rebar rebar)
		{
			if (rebar.SingleRebar != null)
			{
				return rebar.SingleRebar.GetBendData();
			}
			else
			{
				return rebar.SystemRebar.GetBendData();
			}
		}

		public static Autodesk.Revit.DB.Line GetDistributionPath
			(this Model.Entity.Rebar rebar)
		{
			if (rebar.SingleRebar != null)
			{
				return rebar.RebarShapeDrivenAccessor.GetDistributionPath();
			}
			else
			{
				return rebar.SystemRebar.GetDistributionPath();
			}
		}

		public static Autodesk.Revit.DB.XYZ GetDistributionDirection
			(this Model.Entity.Rebar rebar)
		{
			if (rebar.SingleRebar != null)
			{
				return rebar.RebarShapeDrivenAccessor.Normal;
			}
			else
			{
				return rebar.SystemRebar.Normal;
			}
		}

		public static List<Autodesk.Revit.DB.Curve> GetCenterCurves(this Model.Entity.Rebar rebar)
		{
			List<Autodesk.Revit.DB.Curve> curves = null;
			var index = ((double)rebar.Quantity / 2).RoundUp() - 1;

			if (rebar.SingleRebar != null)
			{
				curves = rebar.SingleRebar.GetCenterlineCurves(false, false, false,
					Autodesk.Revit.DB.Structure.MultiplanarOption.IncludeOnlyPlanarCurves, index)
					as List<Autodesk.Revit.DB.Curve>;
			}
			else
			{
				curves = rebar.SystemRebar.GetCenterlineCurves(false, false, false)
					as List<Autodesk.Revit.DB.Curve>;
			}

			if (rebar.RebarShape.Name == "TD_O_01")
			{
				var curve = curves.First();
				var arc = curve as Autodesk.Revit.DB.Arc;
				var center1 = arc.Center;
				var radius1 = arc.Radius;
				var radius2 = radius1 - 10.0.milimeter2Feet();
				var basisX = arc.XDirection;
				var basisY = arc.YDirection;
				var center2 = center1 - basisX * 10.0.milimeter2Feet();

				var extAngle = ((radius1 * radius1 * Math.PI) / 5) / radius1;

				var arc1 = Autodesk.Revit.DB.Arc.Create
					(center1, radius1, -extAngle, Math.PI, basisX, basisY);
				var arc2 = Autodesk.Revit.DB.Arc.Create
					(center2, radius2, Math.PI, Math.PI * 2, basisX, basisY);

				return new List<Curve> { arc1, arc2 };
			}

			return curves;
		}

		public static List<Autodesk.Revit.DB.Curve> GetSupressBendCurves(this Model.Entity.Rebar rebar)
		{
			List<Autodesk.Revit.DB.Curve> curves = null;
			var index = ((double)rebar.Quantity / 2).RoundUp() - 1;

			if (rebar.RebarShape.Name == "TD_O_01")
			{
				curves = rebar.CenterCurves;
			}
			else
			{
				if (rebar.SingleRebar != null)
				{
					curves = rebar.SingleRebar.GetCenterlineCurves(false, false, true,
						Autodesk.Revit.DB.Structure.MultiplanarOption.IncludeOnlyPlanarCurves, index)
						as List<Autodesk.Revit.DB.Curve>;
				}
				else
				{
					curves = rebar.SystemRebar.GetCenterlineCurves(false, false, true)
						as List<Autodesk.Revit.DB.Curve>;
				}
			}

			return curves;
		}

		public static List<string> GetDimensionNames(this Model.Entity.Rebar rebar)
		{
			var def = rebar.RebarShape.GetRebarShapeDefinition();
			string shapeName = rebar.RebarShape.Name;
			List<string> dimNames = shapeName.Contains("TD_02") || shapeName.Contains("TD_01") ? new List<string>() { "A" } : new List<string>();

			if (def is Autodesk.Revit.DB.Structure.RebarShapeDefinitionBySegments)
			{
				var defBySegment = rebar.RebarShape.GetRebarShapeDefinition() as Autodesk.Revit.DB.Structure.RebarShapeDefinitionBySegments;
				for (int i = 0; i < defBySegment.NumberOfSegments; i++)
				{
					var rss = defBySegment.GetSegment(i);
					var constraints = rss.GetConstraints() as List<Autodesk.Revit.DB.Structure.RebarShapeConstraint>;
					foreach (var rsc in constraints)
					{
						if (!(rsc is Autodesk.Revit.DB.Structure.RebarShapeConstraintSegmentLength)) continue;
						ElementId paramId = rsc.GetParamId();
						if ((paramId == ElementId.InvalidElementId)) continue;
						foreach (Parameter p in rebar.RebarShape.Parameters)
						{
							if (p.Id.IntegerValue == paramId.IntegerValue)
							{
								dimNames.Add(p.Definition.Name);
								break;
							}
						}
					}
				}
			}
			if (def is Autodesk.Revit.DB.Structure.RebarShapeDefinitionByArc)
			{
				var defByArc = rebar.RebarShape.GetRebarShapeDefinition()
					as Autodesk.Revit.DB.Structure.RebarShapeDefinitionByArc;

				var constraints = defByArc.GetConstraints();
				foreach (var constraint in constraints)
				{
					ElementId paramId = constraint.GetParamId();
					if ((paramId == ElementId.InvalidElementId))
						continue;
					foreach (Parameter p in rebar.RebarShape.Parameters)
					{
						if (p.Id.IntegerValue == paramId.IntegerValue)
						{
							dimNames.Add(p.Definition.Name);
							break;
						}
					}
				}
			}
			if (shapeName.Contains("TD_02"))
			{
				dimNames.Add("F");
			}
			return dimNames;
		}

		public static List<double> GetDimensionValues(this Model.Entity.Rebar rebar)
		{
			var rrm = revitData.RebarRoundingManager;
			List<double> dimVals = new List<double>();
			var rbd = rebar.RebarBendData;
			if (rbd.HookAngle0 > 0)
			{
				rebar.HookTextIndexs.Add(0);

				double hookLen = 0;
				var bip = BuiltInParameter.REBAR_SHAPE_START_HOOK_LENGTH;
				if (rebar.SingleRebar != null)
				{
					hookLen = rebar.SingleRebar.get_Parameter(bip).AsDouble().feet2Milimeter();
				}
				else
				{
					hookLen = rebar.SystemRebar.get_Parameter(bip).AsDouble().feet2Milimeter();
				}
				dimVals.Add(hookLen);
			}

			if (rebar.SingleRebar != null)
			{
				rebar.DimensionNames.ForEach(x => dimVals.Add(
					Math.Round(rebar.SingleRebar.LookupParameter(x).AsDouble().feet2Milimeter())));
			}
			else
			{
				rebar.DimensionNames.ForEach(x => dimVals.Add(
					Math.Round(rebar.SystemRebar.LookupParameter(x).AsDouble().feet2Milimeter())));
			}

			if (rbd.HookAngle1 > 0)
			{
				rebar.HookTextIndexs.Add(dimVals.Count);

				double hookLen = 0;
				var bip = BuiltInParameter.REBAR_SHAPE_END_HOOK_LENGTH;
				if (rebar.SingleRebar != null)
				{
					hookLen = rebar.SingleRebar.get_Parameter(bip).AsDouble().feet2Milimeter();
				}
				else
				{
					hookLen = rebar.SystemRebar.get_Parameter(bip).AsDouble().feet2Milimeter();
				}
				dimVals.Add(hookLen);
			}

			double roundingNum = rrm.ApplicableSegmentLengthRounding;
			if (roundingNum.IsEqual(0)) roundingNum = 1;
			for (int i = 0; i < dimVals.Count; i++)
			{
				if (rrm.ApplicableSegmentLengthRoundingMethod == RoundingMethod.Nearest)
				{
					dimVals[i] = Math.Round(dimVals[i] / roundingNum) * roundingNum;
				}
				else if (rrm.ApplicableSegmentLengthRoundingMethod == RoundingMethod.Up)
				{
					dimVals[i] = Math.Ceiling(dimVals[i] / roundingNum) * roundingNum;
				}
				else
				{
					dimVals[i] = Math.Floor(dimVals[i] / roundingNum) * roundingNum;
				}
			}
			for (int i = 0; i < dimVals.Count; i++)
			{
				if (dimVals[i].IsEqual(0))
				{
					dimVals[i] = 5000;
				}
			}

			rebar.GetHookLengthIndex(dimVals);

			return dimVals;
		}
		public static void GetHookLengthIndex(this Model.Entity.Rebar rebar, List<double> dimVals)
		{
			var shapeName = rebar.RebarShape.Name;
			if (shapeName.Contains("TD_01"))
			{
				for (int i = 0; i < rebar.DimensionNames.Count; i++)
				{
					if (rebar.DimensionNames[i] == "A")
					{
						rebar.HookTextIndexs.Add(i);
					}
				}
			}
			if (shapeName.Contains("TC_L_01"))
			{
				var index = dimVals.IndexOf(dimVals.Min());
				rebar.HookTextIndexs.Add(index);
			}
			if (shapeName.Contains("TC_L_(Nhấn)_01"))
			{
				for (int i = 0; i < rebar.DimensionNames.Count; i++)
				{
					if (rebar.DimensionNames[i] == "D")
					{
						rebar.HookTextIndexs.Add(i);
					}
				}
			}
			if (shapeName.Contains("TC_L_(Nhấn)_02"))
			{
				for (int i = 0; i < rebar.DimensionNames.Count; i++)
				{
					if (rebar.DimensionNames[i] == "F")
					{
						rebar.HookTextIndexs.Add(i);
					}
				}
			}
			if (shapeName.Contains("TC_L_(Nhấn)_03"))
			{
				for (int i = 0; i < rebar.DimensionNames.Count; i++)
				{
					if (rebar.DimensionNames[i] == "D")
					{
						rebar.HookTextIndexs.Add(i);
					}
				}
			}
			if (shapeName.Contains("TC_L_(Nhấn)_05"))
			{
				for (int i = 0; i < rebar.DimensionNames.Count; i++)
				{
					if (rebar.DimensionNames[i] == "F")
					{
						rebar.HookTextIndexs.Add(i);
					}
				}
			}
			if (shapeName.Contains("TC_UZ_"))
			{
				for (int i = 0; i < rebar.DimensionNames.Count; i++)
				{
					if (rebar.DimensionNames[i] == "A" || rebar.DimensionNames[i] == "D")
					{
						rebar.HookTextIndexs.Add(i);
					}
				}
			}
			if (shapeName.Contains("TC_U_(Nhấn)"))
			{
				for (int i = 0; i < rebar.DimensionNames.Count; i++)
				{
					if (shapeName.Contains("01"))
					{
						if (rebar.DimensionNames[i] == "A" || rebar.DimensionNames[i] == "G")
						{
							rebar.HookTextIndexs.Add(i);
						}
					}
					if (shapeName.Contains("02"))
					{
						if (rebar.DimensionNames[i] == "A" || rebar.DimensionNames[i] == "E")
						{
							rebar.HookTextIndexs.Add(i);
						}
					}
				}
			}
			if (shapeName.Contains("TC_U_01"))
			{
				for (int i = 0; i < rebar.DimensionNames.Count; i++)
				{
					if (rebar.DimensionNames[i] == "B")
					{
						rebar.HookTextIndexs.Add(i);
					}
				}
			}
			if (shapeName.Contains("TC_U_03"))
			{
				for (int i = 0; i < rebar.DimensionNames.Count; i++)
				{
					if (rebar.DimensionNames[i] == "A" || rebar.DimensionNames[i] == "G")
					{
						rebar.HookTextIndexs.Add(i);
					}
				}
			}
			if (shapeName.Contains("TC_U_360_01"))
			{
				for (int i = 0; i < rebar.DimensionNames.Count; i++)
				{
					if (rebar.DimensionNames[i] == "A" || rebar.DimensionNames[i] == "D")
					{
						rebar.HookTextIndexs.Add(i);
					}
				}
			}
			if (shapeName.Contains("TC_U_04"))
			{
				for (int i = 0; i < rebar.DimensionNames.Count; i++)
				{
					if (rebar.DimensionNames[i] == "A" || rebar.DimensionNames[i] == "C")
					{
						rebar.HookTextIndexs.Add(i);
					}
				}
			}
			if (shapeName.Contains("TC_U_360_04"))
			{
				for (int i = 0; i < rebar.DimensionNames.Count; i++)
				{
					if (rebar.DimensionNames[i] == "A" || rebar.DimensionNames[i] == "C")
					{
						rebar.HookTextIndexs.Add(i);
					}
				}
			}
			if (shapeName.Contains("TC_U_360_05"))
			{
				for (int i = 0; i < rebar.DimensionNames.Count; i++)
				{
					if (rebar.DimensionNames[i] == "A" || rebar.DimensionNames[i] == "D")
					{
						rebar.HookTextIndexs.Add(i);
					}
				}
			}
			if (shapeName.Contains("TC_U_360_06"))
			{
				for (int i = 0; i < rebar.DimensionNames.Count; i++)
				{
					if (rebar.DimensionNames[i] == "A" || rebar.DimensionNames[i] == "C")
					{
						rebar.HookTextIndexs.Add(i);
					}
				}
			}
			if (shapeName.Contains("TC_U_360_07"))
			{
				for (int i = 0; i < rebar.DimensionNames.Count; i++)
				{
					if (rebar.DimensionNames[i] == "A" || rebar.DimensionNames[i] == "D")
					{
						rebar.HookTextIndexs.Add(i);
					}
				}
			}
			if (shapeName.Contains("TC_U_360_08"))
			{
				for (int i = 0; i < rebar.DimensionNames.Count; i++)
				{
					if (rebar.DimensionNames[i] == "A" || rebar.DimensionNames[i] == "D")
					{
						rebar.HookTextIndexs.Add(i);
					}
				}
			}
			if (shapeName.Contains("TC_U_360_09"))
			{
				for (int i = 0; i < rebar.DimensionNames.Count; i++)
				{
					if (rebar.DimensionNames[i] == "A" || rebar.DimensionNames[i] == "E")
					{
						rebar.HookTextIndexs.Add(i);
					}
				}
			}

			if (shapeName.Contains("TC_Z_01"))
			{
				for (int i = 0; i < rebar.DimensionNames.Count; i++)
				{
					if (rebar.DimensionNames[i] == "B" || rebar.DimensionNames[i] == "D")
					{
						rebar.HookTextIndexs.Add(i);
					}
				}
			}
			if (shapeName.Contains("TC_Z_04"))
			{
				for (int i = 0; i < rebar.DimensionNames.Count; i++)
				{
					if (rebar.DimensionNames[i] == "A" || rebar.DimensionNames[i] == "C")
					{
						rebar.HookTextIndexs.Add(i);
					}
				}
			}
			if (shapeName.Contains("TC_Z_360") && !shapeName.Contains("TC_Z_360_06"))
			{
				for (int i = 0; i < rebar.DimensionNames.Count; i++)
				{
					if (rebar.DimensionNames[i] == "A" || rebar.DimensionNames[i] == "C")
					{
						rebar.HookTextIndexs.Add(i);
					}
				}
			}
			if (shapeName.Contains("TC_Z_360_06"))
			{
				for (int i = 0; i < rebar.DimensionNames.Count; i++)
				{
					if (rebar.DimensionNames[i] == "A" || rebar.DimensionNames[i] == "D")
					{
						rebar.HookTextIndexs.Add(i);
					}
				}
			}
			if (shapeName.Contains("TC_Z_Nhan") && !shapeName.Contains("TC_Z_Nhan360_02"))
			{
				for (int i = 0; i < rebar.DimensionNames.Count; i++)
				{
					if (rebar.DimensionNames[i] == "A" || rebar.DimensionNames[i] == "E")
					{
						rebar.HookTextIndexs.Add(i);
					}
				}
			}
			if (shapeName.Contains("TC_Z_Nhan360_02") || shapeName.Contains("TC_Z_Nhan90"))
			{
				for (int i = 0; i < rebar.DimensionNames.Count; i++)
				{
					if (rebar.DimensionNames[i] == "A" || rebar.DimensionNames[i] == "D")
					{
						rebar.HookTextIndexs.Add(i);
					}
				}
			}
		}
		public static int GetQuantity(this Model.Entity.Rebar rebar)
		{
			if (rebar.SingleRebar != null)
			{
				return rebar.SingleRebar.Quantity;
			}
			else
			{
				return rebar.SystemRebar.Quantity;
			}
		}

		public static void TransformCurvesForExcelSchedule(this Model.Entity.Rebar rebar)
		{
			var normal = rebar.DistributionDirection;

			var cs = rebar.CenterCurves;
			var supressBendCurves = rebar.SupressBendCurves;

			Autodesk.Revit.DB.XYZ vecX = null, vecY = null;
			double maxLen = 0;
			Line lineX = null;

			if (rebar.RebarShape.Name == "TD_O_01")
			{
				var arc = cs.First() as Autodesk.Revit.DB.Arc;
				vecX = arc.XDirection;
				lineX = Autodesk.Revit.DB.Line.CreateBound(arc.GetEndPoint(0), arc.Center);
			}
			else
			{
				foreach (Curve curve in cs)
				{
					if (curve is Line)
					{
						Line line = curve as Line;
						if (maxLen < line.Length)
						{
							maxLen = line.Length;
							vecX = line.Direction;
							lineX = line.Clone() as Line;
						}
					}
				}
			}

			vecY = normal.CrossProduct(vecX);

			Transform rotateZ = Transform.Identity, rotateX = Transform.Identity;
			if (normal.IsSameOrOppositeDirection(Autodesk.Revit.DB.XYZ.BasisZ))
			{
			}
			else
			{
				var axis = Autodesk.Revit.DB.XYZ.BasisZ.CrossProduct(normal);
				rotateZ = Transform.CreateRotation
					(axis, -normal.AngleTo(Autodesk.Revit.DB.XYZ.BasisZ));
				lineX = lineX.CreateTransformed(rotateZ) as Line;
			}

			rotateX = Transform.CreateRotation
				(Autodesk.Revit.DB.XYZ.BasisZ, -lineX.Direction.AngleTo(Autodesk.Revit.DB.XYZ.BasisX));

			cs = cs.Select(x => x.CreateTransformed(rotateX * rotateZ)).ToList();
			supressBendCurves = supressBendCurves.Select(x => x.CreateTransformed(rotateX * rotateZ)).ToList();

			maxLen = 0;
			foreach (Curve curve in cs)
			{
				if (curve is Line)
				{
					Line line = curve as Line;
					if (maxLen < line.Length)
					{
						maxLen = line.Length;
						vecX = line.Direction;
					}
				}
			}

			var angle = vecX.GetAngle(Autodesk.Revit.DB.XYZ.BasisX, Autodesk.Revit.DB.XYZ.BasisY);
			rotateX = Transform.CreateRotation(Autodesk.Revit.DB.XYZ.BasisZ, -angle);

			rebar.CenterCurves = cs.Select(x => x.CreateTransformed(rotateX)).ToList();
			rebar.SupressBendCurves = supressBendCurves.Select(x => x.CreateTransformed(rotateX)).ToList();
		}

		public static List<Autodesk.Revit.DB.Curve> GetDetailCurves(this Model.Entity.Rebar rebar)
		{
			var curves = new List<Autodesk.Revit.DB.Curve>();
			var shapeName = rebar.RebarShape.Name;

			if (!shapeName.Contains("TD_O_01"))
			{
				return curves;
			}

			//var arc = rebar.CenterCurves[0] as Autodesk.Revit.DB.Arc;
			//var pnt3 = arc.GetEndPoint(1);
			//var center = arc.Center;

			//var line = Autodesk.Revit.DB.Line.CreateBound(center, pnt3);
			//var arc1 = Autodesk.Revit.DB.Arc.Create(center, 20.0.milimeter2Feet(), 0, Math.PI, XYZ.BasisX, XYZ.BasisY);
			//var arc2 = Autodesk.Revit.DB.Arc.Create(center, 20.0.milimeter2Feet(), Math.PI, Math.PI * 2, XYZ.BasisX, XYZ.BasisY);

			//curves.Add(line);
			//curves.Add(arc1);
			//curves.Add(arc2);

			return curves;

		}
	}
}