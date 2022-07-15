using System.Linq;
using SingleData;

namespace Utility
{
	public static class ScheduleColumnUtil
	{
		private static RevitData revitData
		{
			get
			{
				return RevitData.Instance;
			}
		}

		private static RevitModelData revitModelData
		{
			get
			{
				return RevitModelData.Instance;
			}
		}

		public static Model.Entity.ScheduleColumn GetScheduleColumn(int index)
		{
			return revitModelData.ScheduleColumns.Single(x => x.Index == index);
		}
		public static double? GetWidth(this Model.Entity.ScheduleColumn scheduleColumn)
		{
			double width = 0;
			switch (scheduleColumn.Name)
			{
				case "Partition":
					width = 10;
					break;
				case "Type":
					width = 7;
					break;
				case "Rebar Number":
					width = 5.5;
					break;
				case "Shape Image":
					width = 27;
					break;
				case "Quantity":
				case "Bar Length":
				case "Total Bar Length":
				case "Trong Luong/met dai (kg/m)":
				case "HB_SLCauKien":
				case "Khối Lượng (kg)":
					width = 7;
					break;
				case "A":
				case "B":
				case "C":
				case "D":
				case "E":
				case "F":
				case "G":
				case "H":
				case "I":
				case "J":
				case "K":
					width = 5.8;
					break;
				default:
					return null;
			}
			return width;
		}
	}
}
