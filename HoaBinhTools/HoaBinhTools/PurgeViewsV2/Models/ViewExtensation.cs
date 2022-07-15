using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.DB;
using Utils;

namespace HoaBinhTools.PurgeViewsV2.Models
{
	public static class ViewExtensation
	{

		public static bool OnSheet(this List<Viewport> viewports, View view)
		{
			foreach (var Vp in viewports)
			{
				if (Vp.ViewId == view.Id)
				{
					return true;
				}
			}

			return false;
		}


		public static ObservableCollection<View> GetAllViews()
		{
			FilteredElementCollector collector = new FilteredElementCollector(ActiveData.Document);
			collector.OfClass(typeof(View));

			var list = new ObservableCollection<View>();
			foreach (Element e in collector)
			{
				View v = e as Autodesk.Revit.DB.View;

				//if (v.Id.IntegerValue == ActiveData.Document.ActiveView.Id.IntegerValue)
				//	continue;

				if (!v.IsTemplate && v.Name != "Project View" && v.Name != "System Browser" && v.ToString() != "Autodesk.Revit.DB.ViewSheet")
					list.Add(v);
			}
			return list;
		}


		public static List<Autodesk.Revit.DB.View> GetAllViewTemplate()
		{
			FilteredElementCollector collector = new FilteredElementCollector(ActiveData.Document);

			collector.OfClass(typeof(Autodesk.Revit.DB.View));

			List<Autodesk.Revit.DB.View> list = new List<Autodesk.Revit.DB.View>();

			foreach (Element e in collector)
			{
				Autodesk.Revit.DB.View v = e as Autodesk.Revit.DB.View;

				if (v.IsTemplate)
				{
					list.Add(v);
				}
			}

			list = list.OrderBy(x => x.Name).ToList();

			return list;
		}




		private static List<Autodesk.Revit.DB.View> GetAllViewSheet()
		{
			FilteredElementCollector collector = new FilteredElementCollector(ActiveData.Document);

			collector.OfClass(typeof(Autodesk.Revit.DB.View));

			List<Autodesk.Revit.DB.View> list = new List<Autodesk.Revit.DB.View>();

			foreach (Element e in collector)
			{
				Autodesk.Revit.DB.View v = e as Autodesk.Revit.DB.View;
				if (v.ToString() == "Autodesk.Revit.DB.ViewSheet")

					list.Add(v);
			}

			//list = list.OrderBy(x => x.Name).ToList();
			return list;
		}



		private static Dictionary<string, Element> createNodePathLinkLinkRevit()
		{
			Dictionary<string, Element> dictPath = new Dictionary<string, Element>();

			ICollection<ElementId> linksCollection = ExternalFileUtils.GetAllExternalFileReferences(ActiveData.Document);

			foreach (ElementId eleId in linksCollection)
			{
				Element el = ActiveData.Document.GetElement(eleId);

				ExternalFileReference xr = ExternalFileUtils.GetExternalFileReference(ActiveData.Document, eleId);
				ExternalFileReferenceType xrType = xr.ExternalFileReferenceType;

				if (xrType == ExternalFileReferenceType.RevitLink)
				{
					ModelPath xrPath = xr.GetPath();
					string path = ModelPathUtils.ConvertModelPathToUserVisiblePath(xrPath);
					if (!dictPath.ContainsKey("Link Revit/" + path))
						dictPath.Add("Link Revit/" + path, el);
				}
			}
			return dictPath;
		}



		public static ObservableCollection<InfoView> GetInfoViewSheet()
		{
			var AllViewSheet = GetAllViewSheet();

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


		public static ObservableCollection<InfoLink> GetInfoFileLink()
		{
			ObservableCollection<InfoLink> AllFileLink = new ObservableCollection<InfoLink>();

			Categories categories = GetCategories();

			InfoLink cate = new InfoLink();

			cate.LinkType = "Imported Categories";

			var LinksCategory = new ObservableCollection<MyLink>();

			///foreach (Category cat in categories)
			///{
			///	if (CategoryExtenstions.IsImportedCategory(cat))
			///	{
			///		var mylinkcate = new MyLink();
			///		mylinkcate.IsCheck = false;
			///		mylinkcate.Name = cat.Name;
			///		mylinkcate.Id = cat.Id;
			///		LinksCategory.Add(mylinkcate);
			///	}
			///}
			IEnumerable<ImportInstance> CADInstances = new FilteredElementCollector(ActiveData.Document).OfClass(typeof(ImportInstance)).WhereElementIsNotElementType().Cast<ImportInstance>();
			List<ImportInstance> importInstances = CADInstances.Where(x => x != null && !x.IsLinked).ToList();

			foreach (ImportInstance importInstance in importInstances)
			{
				var mylinkcate = new MyLink();
				mylinkcate.IsCheck = false;
				try
				{
					ElementId eid = importInstance.GetTypeId();
					mylinkcate.Name = ActiveData.Document.GetElement(eid).Name;
				}
				catch
				{
					mylinkcate.Name = importInstance.Name;
				}
				
				mylinkcate.Id = importInstance.Id;
				LinksCategory.Add(mylinkcate);
			}

			cate.Links = LinksCategory;

			AllFileLink.Add(cate);



			// Cad

			ICollection<ElementId> linksCollection = ExternalFileUtils.GetAllExternalFileReferences(ActiveData.Document);

			InfoLink linkcad = new InfoLink();

			var LinkFileCads = new ObservableCollection<MyLink>();

			linkcad.LinkType = "Link CAD";

			foreach (ElementId eleId in linksCollection)
			{
				ExternalFileReference xr = ExternalFileUtils.GetExternalFileReference(ActiveData.Document, eleId);
				ExternalFileReferenceType xrType = xr.ExternalFileReferenceType;

				if (xrType == ExternalFileReferenceType.CADLink)
				{
					ModelPath xrPath = xr.GetPath();

					string path = ModelPathUtils.ConvertModelPathToUserVisiblePath(xrPath);

					var mylinkCad = new MyLink();

					mylinkCad.IsCheck = false;

					mylinkCad.Name = "Link CAD/" + path;

					mylinkCad.Id = eleId;

					LinkFileCads.Add(mylinkCad);
				}

			}

			linkcad.Links = LinkFileCads;

			AllFileLink.Add(linkcad);


			// Revit


			InfoLink linkRevit = new InfoLink();

			var LinkFileRevits = new ObservableCollection<MyLink>();

			linkRevit.LinkType = "Link Revit";


			foreach (ElementId eleId in linksCollection)
			{

				ExternalFileReference xr = ExternalFileUtils.GetExternalFileReference(ActiveData.Document, eleId);
				ExternalFileReferenceType xrType = xr.ExternalFileReferenceType;

				if (xrType == ExternalFileReferenceType.RevitLink)
				{
					ModelPath xrPath = xr.GetPath();
					string path = ModelPathUtils.ConvertModelPathToUserVisiblePath(xrPath);

					var mylinkRevit = new MyLink();

					mylinkRevit.IsCheck = false;

					mylinkRevit.Name = "Link CAD/" + path;

					mylinkRevit.Id = eleId;

					LinkFileRevits.Add(mylinkRevit);

				}
			}

			linkRevit.Links = LinkFileRevits;
			AllFileLink.Add(linkRevit);



			return AllFileLink;
		}


		private static Categories GetCategories()
		{
			return ActiveData.Document.Settings.Categories;
		}


		public static ObservableCollection<InfoView> GetInfoViewTemplate()
		{
			var AllViewTemplate = GetAllViewTemplate();

			var elements = AllViewTemplate.Select(e => e.ViewType.ToString()).Distinct().ToList();

			ObservableCollection<InfoView> ObInfo = new ObservableCollection<InfoView>();

			foreach (var VT in elements)
			{
				InfoView viewinfo = new InfoView();

				viewinfo.ViewType = VT.ToString();

				var Views = AllViewTemplate.Where(e => e.ViewType.ToString() == VT);

				foreach (var v in Views)
				{
					viewinfo.Views.Add(new MyView(v));
				}

				ObInfo.Add(viewinfo);
			}


			var MainInfo = new InfoView();

			MainInfo.ViewType = "Views Template";

			MainInfo.IsCheckAll = false;

			MainInfo.Models = ObInfo;

			return new ObservableCollection<InfoView>() { MainInfo };
		}


		public static ObservableCollection<InfoView> GetInfoView()
		{

			var AllView = GetAllViews();

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
	}


	public static class CategoryExtenstions
	{
		public static bool IsImportedCategory(this Category category)
		{
			if (Enum.IsDefined(typeof(BuiltInCategory), category.Id.IntegerValue))
				return false;

			if (category.Id.IntegerValue < -1)
				return false;

			return category.AllowsBoundParameters == false &&
				category.CanAddSubcategory == false &&
				category.HasMaterialQuantities == false &&
				category.IsCuttable == false;
		}
	}

}
