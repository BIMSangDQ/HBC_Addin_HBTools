using Autodesk.Revit.DB;
using Utils;

namespace HoaBinhTools.PurgeViewsV2.Models
{
	public class FillPatternsModel : ViewModelBase
	{
		public ElementId Id { get; set; }

		public FillPatternTarget TypeFill { get; set; }

		public string Name { get; set; }

		public bool isCheck;

		public bool IsCheck
		{
			get
			{
				return isCheck;
			}
			set
			{
				isCheck = value;

				OnPropertyChanged();
			}
		}

		public FillPatternsModel(FillPatternElement ObjectFillPattern)
		{



			this.Id = ObjectFillPattern.Id;

			var Fill = ObjectFillPattern.GetFillPattern();

			this.Name = Fill.Name;

			this.IsCheck = false;

			if (Autodesk.Revit.DB.FillPatternTarget.Drafting == Fill.Target)
			{
				TypeFill = FillPatternTarget.Drafting;
			}
			else if (Autodesk.Revit.DB.FillPatternTarget.Model == Fill.Target)
			{
				TypeFill = FillPatternTarget.Model;
			}
			else
			{
				TypeFill = FillPatternTarget.None;
			}
		}

	}

	public enum FillPatternTarget
	{
		Drafting = 0,
		Model = 1,
		None = 2

	}

}
