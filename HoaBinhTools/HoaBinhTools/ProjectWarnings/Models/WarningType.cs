namespace HoaBinhTools.ProjectWarnings.Models
{
	public class WarningType
	{
		public bool Filter { get; set; }

		public int Count { get; set; }
		public string Description { get; set; }

		public WarningType(bool TF, int count, string D)
		{
			Filter = TF;

			Count = count;

			Description = D;
		}

	}
}

