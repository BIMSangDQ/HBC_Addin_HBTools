using System;
using System.Collections.Generic;
using SingleData;

namespace Utility
{
	public static partial class MassUtil
	{
		private static TemplateData templateData
		{
			get
			{
				return TemplateData.Instance;
			}
		}
		public static Model.Entity.Mass GetMass(this Model.Entity.Element elem,
			Model.Entity.MassType massType)
		{
			var mass = new Model.Entity.Mass { MassType = massType, Element = elem };
			switch (massType)
			{
				case Model.Entity.MassType.Concrete:
					mass.Unit = "m3";
					break;
				case Model.Entity.MassType.Formwork:
					mass.Unit = "m2";
					break;
				case Model.Entity.MassType.Rebar:
					mass.Unit = "kg";
					break;
			}
			return mass;
		}
		public static Model.Entity.Mass GetMassValue(this Model.Entity.Element elem, Model.Entity.MassType massType)
		{
			switch (massType)
			{
				case Model.Entity.MassType.Concrete:
					return elem.GetConcreteVolume();
				case Model.Entity.MassType.Formwork:
					return elem.GetFormworkArea();
				case Model.Entity.MassType.Rebar:
					return elem.GetRebarMass();
			}
			throw new Exception("This code should not be reached!");
		}
		public static List<Model.Entity.Mass> GetMasses(this Model.Entity.Element elem)
		{
			var masses = new List<Model.Entity.Mass>();

			var concreteMass = elem.GetMassValue(Model.Entity.MassType.Concrete);
			if (concreteMass != null)
			{
				masses.Add(concreteMass);
			}

			var formworkMass = elem.GetMassValue(Model.Entity.MassType.Formwork);
			if (formworkMass != null)
			{
				masses.Add(formworkMass);
			}

			templateData.Masses.AddRange(masses);
			return masses;
		}
	}
}
