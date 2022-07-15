using System.Collections.Generic;
using System.Linq;
using SingleData;

namespace Utility
{
	public static partial class DisciplineUtil
	{
		private static TemplateData templateData;
		private static RevitData revitData;
		public static void Initial()
		{
			GetSingleData();

			var structural = new Model.Entity.Discipline
			{
				DisciplineType = Model.Entity.DisciplineType.Structural
			};
			var architect = new Model.Entity.Discipline
			{
				DisciplineType = Model.Entity.DisciplineType.Architect
			};
			var mep = new Model.Entity.Discipline
			{
				DisciplineType = Model.Entity.DisciplineType.MEP
			};

			structural.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_StructuralColumns);
			structural.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_StructuralFraming);
			structural.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_StructuralFoundation);
			structural.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_Floors, "Structural Floors");
			structural.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_Walls, "Structural Walls");

			architect.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_Columns);
			architect.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_CurtainWallPanels);
			architect.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_Doors);
			architect.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_Entourage);
			architect.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_Furniture);
			architect.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_Planting);
			architect.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_Railings);
			architect.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_Site);
			architect.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_Windows);
			architect.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_Floors);
			architect.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_Walls);

			mep.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_CableTrayFitting);
			mep.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_CableTray);
			mep.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_CommunicationDevices);
			mep.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_ConduitFitting);
			mep.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_Conduit);
			mep.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_DataDevices);
			mep.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_DuctFitting);
			mep.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_ElectricalEquipment);
			mep.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_ElectricalFixtures);
			mep.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_FireAlarmDevices);
			mep.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_FlexDuctCurves);
			mep.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_FlexPipeCurves);
			mep.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_LightingDevices);
			mep.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_LightingFixtures);
			mep.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_MechanicalEquipment);
			mep.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_PipeFitting);
			mep.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_PipeCurves);
			mep.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_PlumbingFixtures);
			mep.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_SecurityDevices);
			mep.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_TelephoneDevices);
			mep.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_PipeAccessory);
			mep.AddCategory((int)Autodesk.Revit.DB.BuiltInCategory.OST_SpecialityEquipment);

			templateData.Disciplines = new List<Model.Entity.Discipline>
			{
				structural, architect, mep
			};
		}
		public static Model.Entity.Discipline GetDiscipline(Model.Entity.DisciplineType disciplineType)
		{
			GetSingleData();

			var disciplineView = templateData.Disciplines
				.SingleOrDefault(x => x.DisciplineType == disciplineType);
			return disciplineView;
		}
		public static Model.Entity.Discipline GetValidDiscipline(this Autodesk.Revit.DB.Element elem,
			Model.Entity.Category cate)
		{
			GetSingleData();

			if (cate == null)
				return null;

			switch (cate.BuiltInCategory)
			{
				case Autodesk.Revit.DB.BuiltInCategory.OST_Walls:
				case Autodesk.Revit.DB.BuiltInCategory.OST_Floors:
					var projectData = revitData.ProjectInfo;
					if (projectData.BuildingName == "C18003-CH2")
					{
						return GetDiscipline(Model.Entity.DisciplineType.Structural);
					}
					var isStructural = elem.LookupParameter("Structural").AsInteger();
					return GetDiscipline(isStructural == 1 ?
						Model.Entity.DisciplineType.Structural : Model.Entity.DisciplineType.Architect);
				default:
					return templateData.Disciplines.Single(x => x.Categories.Contains(cate));
			}
		}
		private static void GetSingleData()
		{
			if (Singleton.Instance != null)
			{
				revitData = RevitData.Instance;
				templateData = TemplateData.Instance;
			}
		}
	}
}
