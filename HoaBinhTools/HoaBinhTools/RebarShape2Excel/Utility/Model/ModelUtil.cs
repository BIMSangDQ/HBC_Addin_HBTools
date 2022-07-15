using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Model.Entity;
using SingleData;

namespace Utility
{
	public static class ModelUtil
	{
		private static RevitData revitData
		{
			get
			{
				return RevitData.Instance;
			}
		}

		public static RebarShapeInfo GetCurve2DsFromRebar(Autodesk.Revit.DB.Structure.Rebar rebar)
		{
			var doc = revitData.Document;

			#region General Information

			RebarRoundingManager rrm = revitData.RebarRoundingManager;
			RebarShapeDrivenAccessor rsda = rebar.GetShapeDrivenAccessor();
			RebarShape rbShape = doc.GetElement(rebar.GetShapeId()) as RebarShape;
			RebarBendData rbd = rebar.GetBendData();
			RebarShapeDefinitionBySegments defBySegment
				= rbShape.GetRebarShapeDefinition() as RebarShapeDefinitionBySegments;
			RebarStyle rbStyle = rbShape.RebarStyle;
			List<string> paraNames = new List<string>();
			List<string> paraHookNames = new List<string>();


			List<Curve> cs = rebar.GetCenterlineCurves(false, false, false, MultiplanarOption.IncludeOnlyPlanarCurves, 0) as List<Curve>;
			List<Curve> supressBendCurves = rebar.GetCenterlineCurves(false, false, true, MultiplanarOption.IncludeOnlyPlanarCurves, 0) as List<Curve>;

			var normal = rsda.Normal;
			int numRb = rebar.Quantity;
			double spacRb = 0;
			if (numRb != 1)
			{
				double arrLen = rsda.ArrayLength;
				spacRb = arrLen / (numRb - 1);
			}
			#endregion

			#region Modify Curve for specified Rebar Shape
			Transform fstTranslate = Transform.CreateTranslation(normal * spacRb * numRb / 2);
			cs = cs.ModifyCurve(fstTranslate, rbShape.Name);
			supressBendCurves = supressBendCurves.ModifyCurve(fstTranslate, rbShape.Name);
			#endregion

			#region Get All Parameter defined Contraint in Rebar Family
			for (int i = 0; i < defBySegment.NumberOfSegments; i++)
			{
				RebarShapeSegment rss = defBySegment.GetSegment(i);
				List<RebarShapeConstraint> rscs = rss.GetConstraints() as List<RebarShapeConstraint>;
				foreach (RebarShapeConstraint rsc in rscs)
				{
					if (!(rsc is RebarShapeConstraintSegmentLength))
						continue;
					ElementId paramId = rsc.GetParamId();
					if (paramId == ElementId.InvalidElementId) continue;
					foreach (Parameter p in rbShape.Parameters)
					{
						if (p.Id.IntegerValue == paramId.IntegerValue)
						{
							paraNames.Add(p.Definition.Name);
							break;
						}
					}
				}
			}

			foreach (ElementId paramId in defBySegment.GetParameters())
			{
				if (paramId == ElementId.InvalidElementId) continue;
				Parameter p = rbShape.get_Parameter((BuiltInParameter)paramId.IntegerValue);
				if (!p.IsReadOnly) continue;
				paraHookNames.Add(p.Definition.Name);
			}
			#endregion

			#region Get Constraint in Hook and Parameter
			if (rbd.HookAngle0 > 0)
			{
				foreach (string paraHookName in paraHookNames)
				{
					if (rebar.get_Parameter(BuiltInParameter.REBAR_SHAPE_START_HOOK_LENGTH).AsValueString() == rebar.LookupParameter(paraHookName).AsValueString())
					{
						paraNames.Insert(0, paraHookName); break;
					}
				}
			}

			if (rbd.HookAngle1 > 0)
			{
				foreach (string paraHookName in paraHookNames)
				{
					if (rebar.get_Parameter(BuiltInParameter.REBAR_SHAPE_END_HOOK_LENGTH).AsValueString() == rebar.LookupParameter(paraHookName).AsValueString())
					{
						paraNames.Add(paraHookName); break;
					}
				}
			}
			#endregion

			#region Get a Main Vector of Maximum Length
			Autodesk.Revit.DB.XYZ vecX = null, vecY = null;
			double maxLen = 0;
			Line lineX = null;
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
			vecY = normal.CrossProduct(vecX);
			#endregion

			#region Rotate Curves to 2D Plane (Z=0)
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
			cs = cs.Select(x => x.CreateTransformed(rotateX)).ToList();
			supressBendCurves = supressBendCurves.Select(x => x.CreateTransformed(rotateX)).ToList();

			#endregion

			#region Create Key
			StringBuilder sb = new StringBuilder();
			sb.Append(doc.GetElement(rebar.GetTypeId()).Name);
			sb.Append("__");
			sb.Append(rebar.LookupParameter("Partition").AsString());
			sb.Append("__");
			sb.Append(rebar.LookupParameter("Rebar Number").AsString());
			sb.Append("__");
			string suffix = rebar.LookupParameter("Rebar Number Suffix")?.AsString();
			if (suffix != null && suffix.Length != 0)
			{
				sb.Append(suffix);
			}

			var rebarShape = rebar.LookupParameter("Shape").AsValueString();
			#endregion

			return new RebarShapeInfo(rebar.Id.IntegerValue, cs, supressBendCurves, paraNames, sb.ToString(), rebarShape);
		}
	}
}
