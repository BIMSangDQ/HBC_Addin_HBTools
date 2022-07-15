namespace Model.Entity
{
	public class DimensionInfo
	{
		public string Name { get; set; }
		public string Value { get; set; }
		public DimensionInfo(string name, string value)
		{
			this.Name = name; this.Value = value;
		}
	}
}
