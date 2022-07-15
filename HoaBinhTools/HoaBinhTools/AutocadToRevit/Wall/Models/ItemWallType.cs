using System.Collections.Generic;
using Autodesk.Revit.DB;
using Utils;

namespace HoaBinhTools.AutocadToRevit.Wall.Models
{
	public class ItemWallType : ViewModelBase
	{
		public List<WallType> AllWallTypes { get; set; }

		private WallType wallTypeSelect;

		public WallType WallTypeSelect
		{
			get { return wallTypeSelect; }
			set
			{
				this.wallTypeSelect = value;
				With = MathUtils.FootToMm(WallTypeSelect.Width);
				OnPropertyChanged(nameof(With));
			}
		}

		public double With { get; set; }

		public ItemWallType(WallType WallTypeSl, List<WallType> AllWt)
		{
			AllWallTypes = AllWt;

			WallTypeSelect = WallTypeSl;

			With = MathUtils.FootToMm(WallTypeSelect.Width); ;
		}
	}
}
