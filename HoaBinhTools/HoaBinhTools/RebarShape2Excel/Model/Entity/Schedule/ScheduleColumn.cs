using Utility;


namespace Model.Entity
{
	public class ScheduleColumn
	{
		public string Name { get; set; }
		public int Index { get; set; }
		private double? width;
		public double? Width
		{
			get
			{
				if (width == null)
				{
					width = this.GetWidth();
				}
				return width;
			}
		}
		public bool IsWrapText
		{
			get { return true; }
		}

	}
}
