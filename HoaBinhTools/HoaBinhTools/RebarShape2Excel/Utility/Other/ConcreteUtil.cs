namespace Utility
{
	public static class ConcreteUtil
	{
		public static Model.Entity.Mass GetConcreteVolume(this Model.Entity.Element entElem)
		{
			var elem = entElem.RevitElement;
			var mass = entElem.GetMass(Model.Entity.MassType.Concrete);
			double value = elem.AsValue("Volume").ValueNumber;

			#region Old Code
			//switch ((BuiltInCategory)elem.Category.Id.IntegerValue)
			//{
			//    case BuiltInCategory.OST_StructuralColumns:
			//    case BuiltInCategory.OST_Walls:
			//        value = entElem.Solid.Volume; break;
			//    case BuiltInCategory.OST_Floors:
			//    case BuiltInCategory.OST_StructuralFoundation:
			//        {
			//            var solids = entElem.GetTargetSolidFromElementAndIntersectPaths(
			//                new List<BuiltInCategory> { BuiltInCategory.OST_StructuralColumns,
			//                    BuiltInCategory.OST_Walls }, mass);
			//            if (solids == null)
			//            {
			//                value = 0;
			//            }
			//            else if (solids.Count == 1)
			//            {
			//                value = solids[0].Volume;
			//            }
			//            else
			//            {
			//                value = solids[0].Volume - solids[1].Volume;
			//            }
			//            break;
			//        }
			//    case BuiltInCategory.OST_StructuralFraming:
			//        {
			//            var solids = entElem.GetTargetSolidFromElementAndIntersectPaths(
			//                new List<BuiltInCategory> { BuiltInCategory.OST_StructuralColumns,
			//                    BuiltInCategory.OST_Walls,
			//                BuiltInCategory.OST_Floors,BuiltInCategory.OST_StructuralFoundation}, mass);
			//            if (solids == null)
			//            {
			//                value = 0;
			//            }
			//            else if (solids.Count == 1)
			//            { 
			//                value = solids[0].Volume;
			//            }
			//            else
			//            {
			//                value = solids[0].Volume - solids[1].Volume;
			//            }
			//            break;
			//        }
			//}
			#endregion


			mass.VolumeMassValue = new Model.Entity.MassValue
			{
				Name = "Concrete",
				Value = value,
				Mass = mass
			};

			return mass;
		}
	}
}
