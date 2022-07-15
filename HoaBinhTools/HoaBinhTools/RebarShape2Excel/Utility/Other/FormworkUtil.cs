using System.Collections.Generic;
using Autodesk.Revit.DB;
using SingleData;

namespace Utility
{
	public static class FormworkUtil
	{
		private static List<Autodesk.Revit.DB.BuiltInCategory> formworkCalculate_Categories
			= new List<Autodesk.Revit.DB.BuiltInCategory>
		{
			Autodesk.Revit.DB.BuiltInCategory.OST_StructuralFoundation,
			Autodesk.Revit.DB.BuiltInCategory.OST_StructuralFraming,
			Autodesk.Revit.DB.BuiltInCategory.OST_StructuralColumns,
			Autodesk.Revit.DB.BuiltInCategory.OST_Walls,
			Autodesk.Revit.DB.BuiltInCategory.OST_Floors
		};
		public static bool ValidFaceForFormwork(this Element e, XYZ faceNormal)
		{
			switch ((BuiltInCategory)e.Category.Id.IntegerValue)
			{
				case BuiltInCategory.OST_StructuralColumns:
				case BuiltInCategory.OST_Walls:
					if (GeometryUtil.IsSameOrOppositeDirection(faceNormal, XYZ.BasisZ)) return false;
					break;
				case BuiltInCategory.OST_StructuralFraming:
					if (GeometryUtil.IsSameDirection(faceNormal, XYZ.BasisZ)) return false;
					if (GeometryUtil.IsSameOrOppositeDirection(faceNormal, (e as FamilyInstance).GetTransform().BasisX)) return false;
					break;
				case BuiltInCategory.OST_Floors:
					if (GeometryUtil.IsSameDirection(faceNormal, XYZ.BasisZ)) return false;
					break;
				case BuiltInCategory.OST_StructuralFoundation:
					if (GeometryUtil.IsSameOrOppositeDirection(faceNormal, XYZ.BasisZ)) return false;
					break;
			}
			return true;
		}

		public static void AddSolidToFamilyDocument(Document famDoc, Model.Entity.Element entElem, Solid originSolid, Solid resultSolid)
		{
			var modelData = ModelData.Instance;
			var revitModelData = modelData.RevitModelData;

			foreach (Face face in resultSolid.Faces)
			{
				var normal = face.ComputeNormal(UV.Zero);

				var elem = entElem.RevitElement;
				if (!elem.ValidFaceForFormwork(normal)) continue;
				if (elem is Floor)
				{

				}
				else
				{
					if (!originSolid.IsInOriginialSolidFace(face)) continue;
				}

				var solid = GeometryCreationUtilities.CreateExtrusionGeometry
					(face.GetEdgesAsCurveLoops(), normal, 50.0.milimeter2Feet());
				FreeFormElement.Create(famDoc, solid);
				break;
			}
		}
		public static Model.Entity.MassValue GetFormworkSegment(this Element elem,
			Solid originSolid, Solid resultSolid, Model.Entity.Mass mass)
		{
			var modelData = ModelData.Instance;
			var revitModelData = modelData.RevitModelData;

			var massValue = new Model.Entity.MassValue
			{
				Name = "Formwork",
				Mass = mass
			};

			foreach (Face face in resultSolid.Faces)
			{
				var normal = face.ComputeNormal(UV.Zero);

				if (!elem.ValidFaceForFormwork(normal)) continue;
				if (!originSolid.IsInOriginialSolidFace(face)) continue;

				if (normal.IsPerpendicularDirection(XYZ.BasisZ))
				{
					massValue.Value += face.Area;
				}
				if (normal.IsOppositeDirection(XYZ.BasisZ))
				{
					massValue.Value += face.Area;
				}
			}

			mass.AreaMassValue = massValue;
			return massValue;
		}

		public static Model.Entity.Mass GetFormworkArea(this Model.Entity.Element e1)
		{
			var mass = e1.GetMass(Model.Entity.MassType.Formwork);

			var value = e1.RevitElement.AsValue("Total Area").ValueNumber;
			if (value == 0) return null;

			mass.AreaMassValue = new Model.Entity.MassValue
			{
				Name = "Formwork",
				Mass = mass,
				Value = value,
			};

			//var revitData = RevitData.Instance;
			//var app = revitData.Application;
			//var doc = revitData.Document;
			//var modelData = ModelData.Instance;
			//var revitModelData = modelData.RevitModelData;

			//var solids = e1.GetTargetSolidFromElementAndIntersectPaths
			//    (formworkCalculate_Categories, mass);
			//if (solids == null)
			//{

			//}
			//else
			//{
			//    mass.AreaMassValue = GetFormworkSegment(e1.RevitElement, e1.Solid,
			//        solids[0], mass);
			//}


			return mass;
		}
	}
}
