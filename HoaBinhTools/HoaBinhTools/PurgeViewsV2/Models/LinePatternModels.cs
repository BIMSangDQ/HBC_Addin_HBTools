using Autodesk.Revit.DB;
using Utils;

namespace HoaBinhTools.PurgeViewsV2.Models.PurgeCmd
{
	public class LinePatternModels : ViewModelBase
	{
		public ElementId Id { get; set; }

		public int Index { get; set; }

		public string Name { get; set; }

		private bool isCheck;
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

		public LinePatternModels(LinePatternElement ObjectLinePattern, int Index)
		{

			this.Index = Index;

			Id = ObjectLinePattern.Id;

			Name = ObjectLinePattern.GetLinePattern().Name;

			IsCheck = false;
		}

	}
}
