namespace Model.Entity
{
	public partial class Mass
	{
		public int ID { get; set; }
		public Element Element { get; set; }
		public MassType MassType { get; set; }
		public string Unit { get; set; }
		public Failure Failure { get; set; }

		public virtual MassValue CountMassValue { get; set; }
		public virtual MassValue LengthMassValue { get; set; }
		public virtual MassValue AreaMassValue { get; set; }
		public virtual MassValue VolumeMassValue { get; set; }
		public virtual MassValue WeightMassValue { get; set; }
	}
	public enum MassType
	{
		Concrete, Formwork, Rebar, Paint
	}
}
