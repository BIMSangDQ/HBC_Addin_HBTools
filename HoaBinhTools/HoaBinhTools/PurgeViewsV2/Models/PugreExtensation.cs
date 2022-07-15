using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.DB;
using HoaBinhTools.PurgeViewsV2.Models.PurgeCmd;
using Utils;

namespace HoaBinhTools.PurgeViewsV2.Models
{


	public class ProjectParameterData
	{
		public Definition Definition { get; set; }

		public ElementBinding Binding { get; set; }

		public string Name { get; set; }



	}



	public static class PugreExtensation
	{
		public static ObservableCollection<LinePatternModels> GetLineParrtation()
		{

			ObservableCollection<LinePatternModels> tpm = new ObservableCollection<LinePatternModels>();

			var LinePatter = new FilteredElementCollector(ActiveData.Document).OfClass(typeof(LinePatternElement)).Cast<LinePatternElement>().ToList();


			for (int i = 0; i < LinePatter.Count; i++)
			{
				tpm.Add(new LinePatternModels(LinePatter[i], i + 1));
			}


			return tpm;
		}

		public static ObservableCollection<FillPatternsModel> GetFillPattern()
		{

			ObservableCollection<FillPatternsModel> tpm = new ObservableCollection<FillPatternsModel>();

			List<FillPatternElement> fillp = new FilteredElementCollector(ActiveData.Document).WherePasses(new ElementClassFilter(typeof(FillPatternElement))).OfType<FillPatternElement>().ToList();


			for (int i = 0; i < fillp.Count; i++)
			{

				if (fillp[i].Id == ElementId.InvalidElementId) continue;

				tpm.Add(new FillPatternsModel(fillp[i]));
			}

			return tpm;
		}


		public static ObservableCollection<ParameterModel> GetAllParameter()
		{
			ObservableCollection<ParameterModel> tpm = new ObservableCollection<ParameterModel>();

			if (ActiveData.Document == null)
			{
				throw new ArgumentNullException("doc");
			}

			if (ActiveData.Document.IsFamilyDocument)
			{
				throw new Exception("doc can not be a family document.");
			}

			List<ProjectParameterData> result = new List<ProjectParameterData>();

			BindingMap map = ActiveData.Document.ParameterBindings;

			DefinitionBindingMapIterator it = map.ForwardIterator();

			it.Reset();

			while (it.MoveNext())
			{
				tpm.Add(new ParameterModel(it));
			}

			return tpm;
		}
	}

	internal class MyParameterModelComparer : IEqualityComparer<ParameterModel>
	{



		bool IEqualityComparer<ParameterModel>.Equals(ParameterModel b1, ParameterModel b2)
		{
			if (b2 == null && b1 == null)
				return true;

			else if (b1 == null || b2 == null) return false;


			if (b2.Name == b1.Name)
			{
				return false;
			}

			else
			{
				return true;
			}
		}



		public int GetHashCode(ParameterModel obj)
		{
			return "NguyenChiLinh".GetHashCode();

		}
	}
}
