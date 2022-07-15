using System.Linq;
using SingleData;

namespace Utility
{
	public static partial class IdentifyUtil
	{
		private static TemplateData templateData;

		public static Model.Entity.Identify AddIdentify(Autodesk.Revit.DB.Element elem,
			string docName, Model.Entity.ElementType elemType)
		{
			GetSingleData();

			Model.Entity.Identify identify = null;

			var disciplineType = elemType.Discipline.DisciplineType;

			var bic = (Autodesk.Revit.DB.BuiltInCategory)elem.Category.Id.IntegerValue;

			var block = elem.LookupParameter("HB_Block")?.AsString();
			if (block == null)
			{
				block = elem.LookupParameter("Block")?.AsString();
			}
			var zone = elem.LookupParameter("HB_Zone")?.AsString();
			if (zone == null)
			{
				zone = elem.LookupParameter("Zone")?.AsString();
			}

			Model.Entity.Level level = null;

			level = elem.LevelId?.ToRevitElement()?.Name.GetLevel();
			if (level == null)
			{
				level = elem.LookupParameter("Reference Level")?.AsElementId().ToRevitElement()?.Name.GetLevel();
			}
			if (level == null) throw new Model.Exception.LevelNotValidException();

			switch (disciplineType)
			{
				case Model.Entity.DisciplineType.Structural:
					{
						string name = null;
						switch ((Autodesk.Revit.DB.BuiltInCategory)elem.Category.Id.IntegerValue)
						{
							case Autodesk.Revit.DB.BuiltInCategory.OST_Floors:
								name = elemType.Name; break;
							default:
								name = elem.LookupParameter("HB_ElementName")?.AsString();
								if (name == null)
								{
									name = elem.LookupParameter("ElementName")?.AsString();
								}
								if (name == null)
								{
									name = elem.LookupParameter("Mark")?.AsString();
								}
								if (name?.ToLower() == "gờ") throw new Model.Exception.CaseNotCheckException();
								break;
						}
						name = name == null ? "" : name;

						identify = elemType.Identifies.SingleOrDefault(x => x.Block == block
							&& x.Zone == zone && x.Level == level
							&& x.Name == name);
						if (identify == null)
						{
							identify = new Model.Entity.Identify();
							identify.Block = block;
							identify.Zone = zone;
							identify.Level = level;
							identify.Name = name;
							identify.ElementType = elemType;

							var nav = identify.AddNavigation();
							nav.Elements.Add(elem.GetEntityElement(nav));
							identify.Navigations.Add(nav);
							identify.Count = 1;

							elemType.Identifies.Add(identify);
							templateData.Identifies.Add(identify);
						}
						else
						{
							Model.Entity.Navigation nav = null;
							switch (bic)
							{
								case Autodesk.Revit.DB.BuiltInCategory.OST_StructuralFraming:
									if (identify.IsIdentifyForFraming(elem))
									{
										goto EndOfFunction;
									}
									break;
								case Autodesk.Revit.DB.BuiltInCategory.OST_Floors:
									nav = identify.Navigations[0];
									nav.Elements.Add(elem.GetEntityElement(nav));
									goto EndOfFunction;
							}

							nav = identify.AddNavigation();
							nav.Elements.Add(elem.GetEntityElement(nav));
							identify.Navigations.Add(nav);
							identify.Count++;
						}
					}
					break;
				case Model.Entity.DisciplineType.Architect:
					{
						var roomName = elem.LookupParameter("HB_RoomName")?.AsString();
						var roomNumber = elem.LookupParameter("HB_RoomNumber")?.AsString();

						identify = elemType.Identifies.SingleOrDefault(x => x.Block == block
						&& x.Zone == zone && x.Level == level
						&& x.RoomName == roomName && x.RoomNumber == roomNumber);
						if (identify == null)
						{
							identify = new Model.Entity.Identify();
							identify.Block = block;
							identify.Zone = zone;
							identify.Level = level;
							identify.RoomName = roomName;
							identify.RoomNumber = roomNumber;
							identify.ElementType = elemType;

							var nav = identify.AddNavigation();
							nav.Elements.Add(elem.GetEntityElement(nav));
							identify.Navigations.Add(nav);
							identify.Count = 1;

							elemType.Identifies.Add(identify);
							templateData.Identifies.Add(identify);
						}
						else
						{
							var nav = identify.AddNavigation();
							nav.Elements.Add(elem.GetEntityElement(nav));
							identify.Navigations.Add(nav);
							identify.Count++;
						}
					}
					break;
				case Model.Entity.DisciplineType.MEP:
					{
						var systemType = elem.LookupParameter("System Type")?.AsString();
						var systemName = elem.LookupParameter("System Name")?.AsString();
						var serviceType = elem.LookupParameter("Service Type")?.AsString();

						identify = elemType.Identifies.SingleOrDefault(x => x.Block == block
							&& x.Zone == zone && x.Level == level && x.SystemType == systemType
							&& x.SystemName == systemName && x.ServiceType == serviceType);
						if (identify == null)
						{
							identify = new Model.Entity.Identify();
							identify.Block = block;
							identify.Zone = zone;
							identify.Level = level;
							identify.SystemType = systemType;
							identify.SystemName = systemName;
							identify.ServiceType = serviceType;
							identify.ElementType = elemType;

							var nav = identify.AddNavigation();
							nav.Elements.Add(elem.GetEntityElement(nav));
							identify.Navigations.Add(nav);
							identify.Count = 1;

							elemType.Identifies.Add(identify);
							templateData.Identifies.Add(identify);
						}
						else
						{
							var nav = identify.AddNavigation();
							nav.Elements.Add(elem.GetEntityElement(nav));
							identify.Navigations.Add(nav);
							identify.Count++;
						}
					}
					break;
			}

			EndOfFunction:
			return identify;
		}
		public static bool IsIdentifyForFraming(this Model.Entity.Identify identify,
			Autodesk.Revit.DB.Element elem)
		{
			for (int i = 0; i < identify.Navigations.Count; i++)
			{
				for (int j = 0; j < identify.Navigations[i].Elements.Count; j++)
				{
					if (identify.Navigations[i].Elements[j].RevitElement.IsContinuousBeam(elem))
					{
						var navigation = identify.Navigations[i];
						navigation.Elements.Add(elem.GetEntityElement(navigation));
						return true;
					}
				}
			}

			return false;
		}

		private static void GetSingleData()
		{
			if (Singleton.Instance != null)
			{
				templateData = TemplateData.Instance;
			}
		}
	}
}
