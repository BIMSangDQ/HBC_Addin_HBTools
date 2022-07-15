using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autodesk.Revit.DB;
using Utils;

namespace HoaBinhTools.PurgeViewsV2.Models
{
	public static class PurgeModels
	{
		public static void PurgeFamily()
		{
			IEnumerable<FamilyInstance> Familys = new FilteredElementCollector(ActiveData.Document).OfClass(typeof(FamilyInstance)).ToElements().Cast<FamilyInstance>();

			ProgressBarView EditFami = new ProgressBarView();

			EditFami.Show();

			foreach (FamilyInstance familyInstance in Familys)
			{
				if (!EditFami.Create(Familys.Count(), " Đang Cập Nhật Lại Family ")) break;

				if (familyInstance.Symbol.Family.IsEditable)
				{
					Document familyDoc = familyInstance.Document.EditFamily(familyInstance.Symbol.Family);

					using (Transaction transactionFamily = new Transaction(familyDoc))
					{
						transactionFamily.Start("Edit Family");

						List<ImportInstance> curveElements = new FilteredElementCollector(familyDoc).OfClass(typeof(ImportInstance)).Cast<ImportInstance>().ToList();

						var IDs = curveElements.Select(x => x.Id).ToList();

						if (IDs.Count < 1) continue;

						try
						{
							familyDoc.Delete(IDs);
						}
						catch
						{
						}

						transactionFamily.Commit();
					}

					string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{familyInstance.Symbol.Family.Name}.rfa");

					familyDoc.SaveAs(path, new SaveAsOptions() { OverwriteExistingFile = true, MaximumBackups = 1 });

					familyDoc.Close(true);

					using (Transaction transactionProject = new Transaction(familyInstance.Document))
					{
						transactionProject.Start("Assign");

						familyInstance.Document.LoadFamily(path, new LoadFamilyOptionUtil(), out Family family);

						FamilySymbol newFamilySymbol = new FilteredElementCollector(familyInstance.Document).WherePasses(new FamilySymbolFilter(family.Id)).FirstElement() as FamilySymbol;

						newFamilySymbol.Activate();

						familyInstance.Symbol = newFamilySymbol;

						transactionProject.Commit();
					}

					try
					{
						File.Delete(path);
					}
					catch
					{
					}
				}
			}

			EditFami.Close();
		}


		public static void PurgeUnused()
		{
			string desiredRule = "Project contains unused families and types";

			PerformanceAdviser perfAdviser = PerformanceAdviser.GetPerformanceAdviser();

			IList<PerformanceAdviserRuleId> allRulesList = perfAdviser.GetAllRuleIds();

			IList<PerformanceAdviserRuleId> rulesToExecute = new List<PerformanceAdviserRuleId>();

			//Iterate through each
			foreach (PerformanceAdviserRuleId r in allRulesList)
			{
				if (perfAdviser.GetRuleName(r).Equals(desiredRule))
				{
					rulesToExecute.Add(r);
				}
			}

			IList<FailureMessage> failureMessages = perfAdviser.ExecuteRules(ActiveData.Document, rulesToExecute);

			//Check if there are results
			if (failureMessages.Count() == 0) return;

			ICollection<ElementId> failingElementsIds = failureMessages[0].GetFailingElements();

			foreach (ElementId eid in failingElementsIds)
			{
				try
				{
					ActiveData.Document.Delete(eid);
				}
				catch
				{
				}
			}
		}

	}



	public class LoadFamilyOptionUtil : IFamilyLoadOptions
	{
		public bool OnFamilyFound(bool familyInUse, out bool overwriteParameterValues)
		{
			overwriteParameterValues = true;

			return true;
		}

		public bool OnSharedFamilyFound(Family sharedFamily, bool familyInUse, out FamilySource source, out bool overwriteParameterValues)
		{
			source = FamilySource.Family;

			overwriteParameterValues = true;

			return true;
		}
	}



}
