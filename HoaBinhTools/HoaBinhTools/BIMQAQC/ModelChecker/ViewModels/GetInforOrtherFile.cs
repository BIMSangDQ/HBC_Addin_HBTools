using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using HoaBinhTools.BIMQAQC.ModelChecker.Models;
using HoaBinhTools.PurgeViewsV2.Models;

namespace HoaBinhTools.BIMQAQC.ModelChecker.ViewModels
{
	public class GetInforOrtherFile
	{
		private static ObservableCollection<View> GetAllViews(Document document)
		{
			FilteredElementCollector collector = new FilteredElementCollector(document);
			collector.OfClass(typeof(View));

			IEnumerable<View> views = new FilteredElementCollector(document)
				.WhereElementIsNotElementType()
				.OfClass(typeof(View))
				.Cast<View>()
				.Where(v1 => !v1.IsTemplate && v1.ToString() != "Autodesk.Revit.DB.ViewSheet");

			var list = new ObservableCollection<View>(views);

			FilteredElementCollector collectorViewPort = new FilteredElementCollector(document);
			collectorViewPort.OfClass(typeof(Viewport));

			List<Viewport> viewport = new FilteredElementCollector(document)
			.WhereElementIsNotElementType()
			.OfClass(typeof(Viewport))
			.Cast<Viewport>().ToList();

			List<ElementId> ViewOnSheetId = viewport.Select(v => v.ViewId).ToList();

			ViewOnSheetId = ViewOnSheetId.Distinct().ToList();

			ObservableCollection <View> ListViews = new ObservableCollection<View>(list);
			foreach (View view in list)
			{
				if (ViewOnSheetId.Contains(view.Id) == false)
				{
					ListViews.Add(view);
				}
			}

			//foreach (View view in ListViews)
			//{
			//	Debug.WriteLine(view.Name);
			//}

			return ListViews;
		}

		public static ObservableCollection<InfoView> GetInfoView(Document document)
		{

			var AllView = GetAllViews(document);

			var elements = AllView.Select(e => e.ViewType.ToString()).Distinct().ToList();

			ObservableCollection<InfoView> ObInfo = new ObservableCollection<InfoView>();


			foreach (var VT in elements)
			{
				InfoView viewinfo = new InfoView();

				viewinfo.ViewType = VT.ToString();

				var Views = AllView.Where(e => e.ViewType.ToString() == VT);

				foreach (var v in Views)
				{
					viewinfo.Views.Add(new MyView(v));
				}

				ObInfo.Add(viewinfo);
			}


			var MainInfo = new InfoView();

			MainInfo.ViewType = "Views";

			MainInfo.IsCheckAll = false;

			MainInfo.Models = ObInfo;

			return new ObservableCollection<InfoView>() { MainInfo };
		}

		private static List<View> GetAllViewSheet(Document document)
		{
			FilteredElementCollector collector = new FilteredElementCollector(document);

			collector.OfClass(typeof(View));

			List<View> list = new List<View>();

			foreach (Element e in collector)
			{
				View v = e as View;
				if (v.ToString() == "Autodesk.Revit.DB.ViewSheet")

					list.Add(v);
			}

			return list;
		}

		public static ObservableCollection<InfoView> GetInfoViewSheet(Document document)
		{
			var AllViewSheet = GetAllViewSheet(document);

			var elements = AllViewSheet.Select(e => e.ViewType.ToString()).Distinct().ToList();

			ObservableCollection<InfoView> ObInfo = new ObservableCollection<InfoView>();

			foreach (var VT in elements)
			{
				InfoView viewinfo = new InfoView();

				viewinfo.ViewType = VT.ToString();

				var Views = AllViewSheet.Where(e => e.ViewType.ToString() == VT);

				foreach (var v in Views)
				{
					viewinfo.Views.Add(new MyView(v));
				}

				ObInfo.Add(viewinfo);
			}

			var MainInfo = new InfoView();

			MainInfo.ViewType = "Views Sheet";

			MainInfo.IsCheckAll = false;

			MainInfo.Models = ObInfo;

			return new ObservableCollection<InfoView>() { MainInfo };

		}

		public static int GetFamiliesInPlace(Document document)
		{
			List<FamilyInstance> ListFamily = new FilteredElementCollector(document).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList();

			ListFamily = ListFamily.Where(x => x.Symbol.Family.IsInPlace).ToList();
			return ListFamily.Count;
		}

		public static int GetDuplicateIntances(Document document)
		{
			string desiredRule = "Duplicate instances";

			PerformanceAdviser perfAdviser = PerformanceAdviser.GetPerformanceAdviser();

			IList<PerformanceAdviserRuleId> allRulesList = perfAdviser.GetAllRuleIds();

			IList<PerformanceAdviserRuleId> rulesToExecute = new List<PerformanceAdviserRuleId>();

			List<string> rule = new List<string>();

			foreach (PerformanceAdviserRuleId r in allRulesList)
			{
				rule.Add(perfAdviser.GetRuleName(r));
				if (perfAdviser.GetRuleName(r).Equals(desiredRule))
				{
					rulesToExecute.Add(r);
				}
			}

			IList<FailureMessage> failureMessages = perfAdviser.ExecuteRules(document, rulesToExecute);

			if (failureMessages.Count() == 0) return 0;

			ICollection<ElementId> failingElementsIds = failureMessages[0].GetFailingElements();

			int i =0;
			List<ElementId> failingElements = new List<ElementId>();

			foreach (FailureMessage failureMessage in failureMessages)
			{
				//Debug.WriteLine(failureMessage.GetDescriptionText());
				i=i+failureMessage.GetFailingElements().Count();
				
			}

			return i;
		}

		public static int CountPurgeElement(Document document)
		{
			string desiredRule = "Project contains unused families and types";

			PerformanceAdviser perfAdviser = PerformanceAdviser.GetPerformanceAdviser();

			IList<PerformanceAdviserRuleId> allRulesList = perfAdviser.GetAllRuleIds();

			IList<PerformanceAdviserRuleId> rulesToExecute = new List<PerformanceAdviserRuleId>();

			foreach (PerformanceAdviserRuleId r in allRulesList)
			{
				if (perfAdviser.GetRuleName(r).Equals(desiredRule))
				{
					rulesToExecute.Add(r);
				}
			}

			IList<FailureMessage> failureMessages = perfAdviser.ExecuteRules(document, rulesToExecute);

			//Check if there are results
			if (failureMessages.Count() == 0) return 0;

			int i = 0;
			foreach (FailureMessage failureMessage in failureMessages)
			{
				i = i + failureMessage.GetFailingElements().Count();
			}

			return i;
		}

		public static int GetHiddenElement(Document document)
		{
			List<View> views = new FilteredElementCollector(document)
							.WhereElementIsNotElementType()
							.OfClass(typeof(View))
							.Cast<View>()
							.Where(v1 => !v1.IsTemplate && v1.ToString() != "Autodesk.Revit.DB.ViewSheet" && v1.ToString() != "Autodesk.Revit.DB.ViewDrafting")
							.Where(v1 => v1.CanUseTemporaryVisibilityModes()).ToList();

			List<Element> elements = new List<Element>();

			int i = 0;

			elements = new FilteredElementCollector(document)
				.WhereElementIsNotElementType().Where(x=> null != x.Category && x.Category.HasMaterialQuantities).ToList();

			foreach (Element element in elements)
			{
				foreach (View view in views)
				{
					if (element.IsHidden(view))
					{
						i = i + 1;
						break;
					}
				}
			}

			return i;
		}

		public static string GetfileName(Document document)
		{
			string fname = "";
			try
			{
				var modelPath = document.GetWorksharingCentralModelPath();

				var centralServerPath = ModelPathUtils.ConvertModelPathToUserVisiblePath(modelPath);

				fname = centralServerPath.ToString();
			}
			catch
			{
				fname = document.PathName;
			}

			return fname;
		}

		public static ObservableCollection<ClassFailureMessages> Test(Document document)
		{
			ObservableCollection<ClassFailureMessages> classFailureMessages = new ObservableCollection<ClassFailureMessages>();

			PerformanceAdviser perfAdviser = PerformanceAdviser.GetPerformanceAdviser();

			IList<PerformanceAdviserRuleId> allRulesList = perfAdviser.GetAllRuleIds();

			foreach (PerformanceAdviserRuleId r in allRulesList)
			{
				IList<PerformanceAdviserRuleId> rulesToExecute = new List<PerformanceAdviserRuleId>();
				rulesToExecute.Add(r);

				IList<FailureMessage> failureMessages = perfAdviser.ExecuteRules(document, rulesToExecute);

				List<ElementId> elementIds = new List<ElementId>();
				foreach (FailureMessage failureMessage in failureMessages)
				{
					elementIds = failureMessage.GetFailingElements().ToList();
				}

				List<ElementFailure> elementFailures = new List<ElementFailure>();
				foreach (ElementId Id in elementIds)
				{
					Element e = document.GetElement(Id);
					Category category = null;
					try
					{
						category = e.Category;
					}
					catch (Exception ex)
					{ }

					elementFailures.Add(new ElementFailure
					{
						ElementId = Id,
						Category = category,
					});
				}

				classFailureMessages.Add(new ClassFailureMessages
				{
					Document = GetfileName(document),
					DesiredRule = perfAdviser.GetRuleName(r),
					ListElement = elementFailures
				});

			}

			return classFailureMessages;
		}
	}
}
