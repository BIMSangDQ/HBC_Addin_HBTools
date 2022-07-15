using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Utils;

namespace HoaBinhTools.FramingInformation.Models
{
	public static class FramingInfoView3D
	{
		public static View ViewHB { get; set; } = null;

		public static View GetViewHB()
		{

			if (ViewHB == null)
			{
				ViewHB = new FilteredElementCollector(ActiveData.Document).OfClass(typeof(View)).Cast<View>().FirstOrDefault<View>(x => x.Name == "(BIM Export)Beams Information Modeling " + ActiveData.Application.Username) as View3D;

				if (ViewHB == null)
				{
					return CreateView3D("(BIM Export)Beams Information Modeling " + ActiveData.Application.Username);
				}
				else
				{
					return ViewHB;
				}
			}
			else
			{
				return ViewHB;
			}

		}


		public static void IsolateFraming(this View view, Document doc, List<ElementId> Light)
		{
			if (view.IsTemporaryHideIsolateActive())
			{
				TemporaryViewMode tempView = TemporaryViewMode.TemporaryHideIsolate;

				view.DisableTemporaryViewMode(tempView);
			}

			view.IsolateElementsTemporary(Light.ToList());

			// zom tới telemt
			ActiveData.UIDoc.ShowElements(Light.ToList());
		}


		public static View CreateView3D(string IsolateFraming)
		{
			View3D view3d = null;

			if (!HasView(IsolateFraming))
			{
				ViewFamilyType viewFamilyType3D = new FilteredElementCollector(ActiveData.Document).OfClass(typeof(ViewFamilyType)).Cast<ViewFamilyType>().FirstOrDefault<ViewFamilyType>(x => ViewFamily.ThreeDimensional == x.ViewFamily);

				view3d = (viewFamilyType3D != null) ? View3D.CreateIsometric(ActiveData.Document, viewFamilyType3D.Id) : null;

				view3d.Name = IsolateFraming;
			}
			else
			{
				view3d = GetViewHB() as View3D;
			}

			ActiveData.UIDoc.UpdateAllOpenViews();

			ActiveData.UIDoc.RefreshActiveView();

			ViewHB = view3d;

			return view3d;
		}


		public static bool HasView(string Name)
		{
			var hasview = new FilteredElementCollector(ActiveData.Document).OfClass(typeof(View)).Cast<View>().FirstOrDefault<View>(x => x.Name == Name);

			return hasview != null ? true : false;
		}

		public static void ActivewHB()
		{
			ActiveData.UIDoc.ActiveView = GetViewHB();


		}

		public static void CloseViewHB()
		{
			foreach (UIView uiv in ActiveData.UIDoc.GetOpenUIViews())
			{
				if (uiv.ViewId == GetViewHB().Id)
				{
					uiv.Close();

					break;
				}

			}
		}

	}

}
