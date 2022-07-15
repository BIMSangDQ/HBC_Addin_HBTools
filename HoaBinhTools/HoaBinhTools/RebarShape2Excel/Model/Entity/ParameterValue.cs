namespace Model
{
	public class ParameterValue
	{
		public ParameterValueType ParameterValueType { get; set; }
		public string ValueText { get; set; }
		public double ValueNumber { get; set; }
		public string Value
		{
			get
			{
				switch (ParameterValueType)
				{
					case ParameterValueType.Text: return ValueText;
					case ParameterValueType.Number: return ValueNumber.ToString();
				}
				throw new System.Exception("Đoạn code này không được đọc tới.");
			}
		}
	}
	public enum ParameterValueType
	{
		Text, Number
	}
}