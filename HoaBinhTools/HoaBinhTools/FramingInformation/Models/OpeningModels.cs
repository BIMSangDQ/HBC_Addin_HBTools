namespace HoaBinhTools.FramingInformation.Models
{
	public class OpeningModels
	{

		public ShapeOpening Shape { get; set; }

		public double Height { get; set; }

		public double Width { get; set; }

		public double DistanceX { get; set; }

		public double DistanceY { get; set; }

		public enum ShapeOpening
		{
			HCN = 1,

			HT = 2,
		}

	}

}
