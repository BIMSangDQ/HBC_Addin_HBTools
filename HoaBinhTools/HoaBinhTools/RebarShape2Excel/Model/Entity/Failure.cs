using System.Collections.Generic;

namespace Model.Entity
{
	public class Failure
	{
		public int ID { get; set; }
		public Failure(FailureType failureType)
		{
			switch (failureType)
			{
				case FailureType.CalculatingGeometry:
					{
						LongDescription = "Calculating Geometry Error! This may be due to geometric inaccuracies in the solids, such as slightly misaligned faces or edges.";
						BriefDescription = "Calculating Geometry Error";
					}
					break;
				case FailureType.Unknown:
					{
						LongDescription = "Unknown Error!";
						BriefDescription = "Unknown Error";
					}
					break;
			}
		}
		public FailureType FailureType { get; set; }
		public string LongDescription { get; set; }
		public string BriefDescription { get; set; }
		public List<Element> Elements { get; set; } = new List<Element>();
	}

	public enum FailureType
	{
		CalculatingGeometry, Unknown
	}
}
