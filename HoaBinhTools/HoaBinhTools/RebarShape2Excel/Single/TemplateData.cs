using System;
using System.Collections.Generic;

namespace SingleData
{
	public class TemplateData
	{
		public static TemplateData Instance { get { return Singleton.Instance.TemplateData; } }

		private List<Model.Entity.Failure> failures;
		public virtual List<Model.Entity.Failure> Failures
		{
			get
			{
				if (failures == null)
					failures = new List<Model.Entity.Failure>();
				return failures;
			}
		}

		private List<Model.Entity.Level> levels;
		public virtual List<Model.Entity.Level> Levels
		{
			get
			{
				if (levels == null)
					levels = new List<Model.Entity.Level>();
				return levels;
			}
		}

		private List<Model.Entity.Discipline> disciplines;
		public List<Model.Entity.Discipline> Disciplines
		{
			get
			{
				return disciplines;
			}
			set
			{
				disciplines = value;
			}
		}

		private List<Model.Entity.Category> categories = new List<Model.Entity.Category>();
		public List<Model.Entity.Category> Categories
		{
			get
			{
				return categories;
			}
			set
			{
				categories = value;
			}
		}

		private List<Model.Entity.Family> families = new List<Model.Entity.Family>();
		public virtual List<Model.Entity.Family> Families
		{
			get
			{
				return families;
			}
			set
			{
				families = value;
			}
		}

		private List<Model.Entity.ElementType> elementTypes = new List<Model.Entity.ElementType>();
		public virtual List<Model.Entity.ElementType> ElementTypes
		{
			get
			{
				return elementTypes;
			}
			set
			{
				elementTypes = value;
			}
		}

		private List<Model.Entity.Element> elements = new List<Model.Entity.Element>();
		public virtual List<Model.Entity.Element> Elements
		{
			get
			{
				return elements;
			}
			set
			{
				elements = value;
			}
		}

		private List<Model.Entity.XYZ> xyzs = new List<Model.Entity.XYZ>();
		public virtual List<Model.Entity.XYZ> XYZs
		{
			get
			{
				return xyzs;
			}
			set
			{
				xyzs = value;
			}
		}

		private List<Model.Entity.Identify> identifies = new List<Model.Entity.Identify>();
		public virtual List<Model.Entity.Identify> Identifies
		{
			get
			{
				return identifies;
			}
			set
			{
				identifies = value;
			}
		}

		private List<Model.Entity.Navigation> navigations = new List<Model.Entity.Navigation>();
		public virtual List<Model.Entity.Navigation> Navigations
		{
			get
			{
				return navigations;
			}
			set
			{
				navigations = value;
			}
		}

		private List<Model.Entity.Mass> masses = new List<Model.Entity.Mass>();
		public virtual List<Model.Entity.Mass> Masses
		{
			get
			{
				return masses;
			}
			set
			{
				masses = value;
			}
		}

		public virtual Model.Entity.Project Project { get; set; }

		public virtual string EditValueElementInGroupWarningName { get; set; }
		   = "A group has been changed outside group edit mode";

		public virtual Guid Edit_Group_Outside_Error { get; set; }
			= new Guid("c519d291-2e28-4075-b0d0-3cbc8c848bc2");
		public virtual Guid Dimension_Not_Valid_Error { get; set; }
			= new Guid("b44c8ba0-7a86-44c1-bbf1-2de8e2017266");

		public Action<Autodesk.Revit.DB.FailureMessageAccessor> WarningMessageAccessor { get; set; }
			= x => { };
		public Action<Autodesk.Revit.DB.FailureMessageAccessor> ErrorMessageAccessor { get; set; }
			= x => { };
	}
}
