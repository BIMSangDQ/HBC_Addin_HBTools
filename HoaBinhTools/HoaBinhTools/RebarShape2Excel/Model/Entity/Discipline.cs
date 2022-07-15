using System.Collections.Generic;

namespace Model.Entity
{
	public class Discipline
	{
		public DisciplineType DisciplineType { get; set; }
		public List<Category> Categories { get; set; } = new List<Category>();
	}
	public enum DisciplineType
	{
		Structural, Architect, MEP
	}
}
