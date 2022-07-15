using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using HoaBinhTools.FramingInformation.Models;
using HoaBinhTools.FramingInformation.ViewModels;
using HoaBinhTools.QLUser.Models;
using Utils;

namespace HoaBinhTools.FramingInformation
{
	[Transaction(TransactionMode.Manual)]
	public class FramingInformationCmd : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			// Show Form
			UIApplication uiapp = commandData.Application;

			UIDocument uidoc = uiapp.ActiveUIDocument;

			Application app = uiapp.Application;

			String assemblyFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
			String assemplyDirPath = System.IO.Path.GetDirectoryName(assemblyFilePath);

			#region Ghi nhận tần suất 05/07/2021
			string nameaddin = "Rebar Beam";
			Registration r = new Registration();

			Thread t = new Thread(() =>
			{
				r.GhiNhanTanSuat(nameaddin);
			});
			t.Start();
			#endregion

			ActiveData.GetInformation(uidoc);

			List<List<Element>> refeFraming = new List<List<Element>>();

			if (HoaBinhTools.Properties.Settings.Default.PickObject)
			{
				while (true)
				{
					try
					{
						var EleFami = ActiveData.Selection.PickObjects(ObjectType.Element, new FilterCategoryUtils { FuncElement = x => x.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming && ((x as FamilyInstance) == null ? false : !((x as FamilyInstance).Symbol.Family.IsInPlace)) }).ToList().ToElements();

						if (EleFami.Count == 0)
						{
							continue;
						}

						refeFraming.Add(EleFami);
					}
					catch
					{
						break;
					}
				}

			}
			else
			{
				var Eles = ActiveData.Selection.PickObjects(ObjectType.Element, new FilterCategoryUtils { FuncElement = x => x.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming && ((x is FamilyInstance) ? !((x as FamilyInstance).Symbol.Family.IsInPlace) : false) }).ToList().ToElements();

				refeFraming = Eles.GetGroupContiuousBeam();
			}



			if (refeFraming.Count == 0)
			{
				return Result.Cancelled;
			}



			ExternalEventHandler.Instance.Create();

			using (Transaction transGroup = new Transaction(ActiveData.Document))
			{
				transGroup.Start("Framing");

				FramingInfoViewModels Fs = new FramingInfoViewModels(refeFraming);

				Fs.Run();

				transGroup.Commit();
			}


			FramingInfoView3D.ActivewHB();

			return Result.Succeeded;
		}
	}
}
