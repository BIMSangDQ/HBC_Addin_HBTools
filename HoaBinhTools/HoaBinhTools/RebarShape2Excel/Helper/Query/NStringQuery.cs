namespace Helper
{
	public class NStringQuery
	{
		public NStringQuery(string value)
		{
			Value = $"N\'{value}\'";
		}
		public string Value { get; set; }
	}
}
