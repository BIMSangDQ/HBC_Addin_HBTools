#region Using
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
#endregion

namespace Utils
{
	public interface IElementSelectionFilter : ISelectionFilter { }
	public interface IReferenceSelectionFilter : ISelectionFilter { }
	public interface ILogicalCombinationFilter : ISelectionFilter
	{
		bool ExecuteAll { get; set; }
	}
	public static class SelectionFilter
	{

		#region Elements

		#region Methods
		public static IElementSelectionFilter GetElementFilter<T>()
		{
			return new ElementTypeFilter(typeof(T));
		}
		public static IElementSelectionFilter GetElementLinkFilter<T>()
		{
			return new ElementLinkTypeFilter(typeof(T));
		}
		public static IElementSelectionFilter GetElementFilter(IEnumerable<Type> allowedTypes)
		{
			return new ElementTypeFilter(allowedTypes);
		}
		public static IElementSelectionFilter GetElementLinkFilter(IEnumerable<Type> allowedTypes)
		{
			return new ElementLinkTypeFilter(allowedTypes);
		}
		public static IElementSelectionFilter GetElementFilter(ElementFilter filter)
		{
			return new ElementFilterFilter(filter);
		}
		public static IElementSelectionFilter GetElementLinkFilter(ElementFilter filter)
		{
			return new ElementLinkFilterFilter(filter);
		}
		public static IElementSelectionFilter GetElementFilter(Func<Element, bool> filterMethod)
		{
			return new DelegatesFilter(filterMethod, DelegatesFilter.NoReferences);
		}
		public static IElementSelectionFilter GetElementFilter(ElementId id)
		{
			return new ElementIdFilter(id);
		}
		public static IElementSelectionFilter GetElementFilter(IEnumerable<ElementId> ids)
		{
			return new ElementIdFilter(ids);
		}
		#endregion

		#region Abstract class
		private abstract class ElementSelectionFilter : IElementSelectionFilter, IReferenceSelectionFilter, ISelectionFilter
		{
			public abstract bool AllowElement(Element elem);
			public bool AllowReference(Reference reference, XYZ position)
			{
				return false;
			}
		}
		private abstract class ElementLinkSelectionFilter : IElementSelectionFilter, IReferenceSelectionFilter, ISelectionFilter
		{
			protected Document Document { get; set; }
			public bool AllowElement(Element elem)
			{
				if (elem is RevitLinkInstance revitLinkInstance)
				{
					Document = revitLinkInstance.GetLinkDocument();
					return true;
				}
				return false;
			}
			public abstract bool AllowReference(Reference reference, XYZ position);
		}
		#endregion

		#region Class
		private class ElementTypeFilter : ElementSelectionFilter, IElementSelectionFilter, IReferenceSelectionFilter, ISelectionFilter
		{
			private readonly List<Type> types = new List<Type>();
			public ElementTypeFilter(Type type)
			{
				types.Add(type);
			}
			public ElementTypeFilter(IEnumerable<Type> types)
			{
				this.types.AddRange(types);
			}
			public override bool AllowElement(Element element)
			{
				return types.Any(type => type.IsInstanceOfType(element));
			}
		}
		private class ElementIdFilter : ElementSelectionFilter, IElementSelectionFilter, IReferenceSelectionFilter, ISelectionFilter
		{
			private readonly List<ElementId> ids = new List<ElementId>();
			public ElementIdFilter(ElementId id)
			{
				this.ids.Add(id);
			}
			public ElementIdFilter(IEnumerable<ElementId> ids)
			{
				this.ids.AddRange(ids);
			}
			public override bool AllowElement(Element elem)
			{
				return ids.Contains(elem.Id);
			}
		}
		private class ElementFilterFilter : ElementSelectionFilter, IElementSelectionFilter, IReferenceSelectionFilter, ISelectionFilter
		{
			private readonly ElementFilter filter;
			public ElementFilterFilter(ElementFilter filter)
			{
				this.filter = filter;
			}
			public override bool AllowElement(Element elem)
			{
				return filter.PassesFilter(elem);
			}
		}
		public class DelegatesFilter : IElementSelectionFilter, IReferenceSelectionFilter, ISelectionFilter
		{
			private readonly Func<Element, bool> elementFilter;
			private readonly Func<Reference, XYZ, bool> referenceFilter;
			public DelegatesFilter(Func<Element, bool> elementFilter, Func<Reference, XYZ, bool> referenceFilter)
			{
				this.elementFilter = elementFilter;
				this.referenceFilter = referenceFilter;
			}
			public static Func<Element, bool> NoElements
			{
				get { return element => false; }
			}
			public static Func<Element, bool> AllElements
			{
				get { return element => true; }
			}
			public static Func<Reference, XYZ, bool> NoReferences
			{
				get { return (reference, xyz) => false; }
			}
			public static Func<Reference, XYZ, bool> AllReferences
			{
				get { return (reference, xyz) => true; }
			}
			public bool AllowElement(Element elem)
			{
				return elementFilter(elem);
			}
			public bool AllowReference(Reference reference, XYZ position)
			{
				return referenceFilter(reference, position);
			}
		}
		private class ElementLinkTypeFilter : ElementLinkSelectionFilter, IElementSelectionFilter, IReferenceSelectionFilter, ISelectionFilter
		{
			private readonly List<Type> types = new List<Type>();
			public ElementLinkTypeFilter(Type type)
			{
				types.Add(type);
			}
			public ElementLinkTypeFilter(IEnumerable<Type> types)
			{
				this.types.AddRange(types);
			}

			public override bool AllowReference(Reference reference, XYZ position)
			{
				return types.Any(type => type.IsInstanceOfType(Document.GetElement(reference.LinkedElementId)));
			}
		}
		private class ElementLinkFilterFilter : ElementLinkSelectionFilter, IElementSelectionFilter, IReferenceSelectionFilter, ISelectionFilter
		{
			private readonly ElementFilter filter;
			public ElementLinkFilterFilter(ElementFilter filter)
			{
				this.filter = filter;
			}

			public override bool AllowReference(Reference reference, XYZ position)
			{
				return filter.PassesFilter(Document.GetElement(reference.LinkedElementId));
			}
		}
		#endregion

		#endregion

		#region Reference
		public static IReferenceSelectionFilter GetReferenceFilter(Func<Reference, XYZ, bool> filterMethod)
		{
			return new DelegatesFilter(DelegatesFilter.NoElements, filterMethod);
		}
		#endregion

		#region Element and reference
		public static ISelectionFilter GetFilter(Func<Element, bool> elementFilterMethod, Func<Reference, XYZ, bool> referencesFilterMethod)
		{
			return new DelegatesFilter(elementFilterMethod, referencesFilterMethod);
		}
		#endregion

		#region Logical filter other
		public static ILogicalCombinationFilter GetLogicalOrFilter(ISelectionFilter first, ISelectionFilter second, bool executeAll = false)
		{
			return new LogicalOrFilter(first, second, executeAll);
		}
		public static ILogicalCombinationFilter GetLogicalOrFilter(ISelectionFilter first, params ISelectionFilter[] filters)
		{
			return new LogicalOrFilter(first, filters);
		}
		public static ILogicalCombinationFilter GetLogicalOrFilter(IEnumerable<ISelectionFilter> filters)
		{
			return new LogicalOrFilter(filters);
		}
		public static ILogicalCombinationFilter GetLogicalAndFilter(ISelectionFilter first, ISelectionFilter second, bool executeAll = false)
		{
			return new LogicalAndFilter(first, second, executeAll);
		}
		public static ILogicalCombinationFilter GetLogicalAndFilter(ISelectionFilter first, params ISelectionFilter[] filters)
		{
			return new LogicalAndFilter(first, filters);
		}
		public static ILogicalCombinationFilter GetLogicalAndFilter(IEnumerable<ISelectionFilter> filters)
		{
			return new LogicalAndFilter(filters);
		}
		public static ISelectionFilter GetLogicalNotFilter(ISelectionFilter filter)
		{
			return new LogicalNotFilter(filter);
		}

		#region abstract class
		private abstract class LogicalCombinationFilter : ISelectionFilter, ILogicalCombinationFilter
		{
			protected readonly List<ISelectionFilter> filters = new List<ISelectionFilter>();
			public bool ExecuteAll { get; set; }
			protected LogicalCombinationFilter(ISelectionFilter first, ISelectionFilter second, bool executeAll = false)
			{
				ExecuteAll = executeAll;
				filters.Add(first);
				filters.Add(second);
			}
			protected LogicalCombinationFilter(ISelectionFilter first, params ISelectionFilter[] filters)
			{
				ExecuteAll = false;
				this.filters.Add(first);
				this.filters.AddRange(filters);
			}
			protected LogicalCombinationFilter(IEnumerable<ISelectionFilter> filters)
			{
				ExecuteAll = false;
				this.filters.AddRange(filters);
			}
			public abstract bool AllowElement(Element elem);
			public abstract bool AllowReference(Reference reference, XYZ position);
		}
		#endregion

		#region class
		private class LogicalOrFilter : LogicalCombinationFilter, ISelectionFilter
		{
			public LogicalOrFilter(ISelectionFilter first, ISelectionFilter second, bool executeAll) : base(first, second, executeAll) { }
			public LogicalOrFilter(ISelectionFilter first, params ISelectionFilter[] filters) : base(first, filters) { }
			public LogicalOrFilter(IEnumerable<ISelectionFilter> filters) : base(filters) { }
			public override bool AllowElement(Element elem)
			{
				bool erg = false;
				if (ExecuteAll)
				{
					filters.ForEach(filter => erg |= filter.AllowElement(elem));
				}
				else
				{
					erg = filters.Any(filter => filter.AllowElement(elem));
				}
				return erg;
			}

			public override bool AllowReference(Reference reference, XYZ position)
			{
				bool erg = false;
				if (ExecuteAll)
				{
					filters.ForEach(filter => erg |= filter.AllowReference(reference, position));
				}
				else
				{
					erg = filters.Any(filter => filter.AllowReference(reference, position));
				}
				return erg;
			}
		}
		private class LogicalAndFilter : LogicalCombinationFilter, ISelectionFilter
		{
			public LogicalAndFilter(ISelectionFilter first, ISelectionFilter second, bool executeAll) : base(first, second, executeAll) { }
			public LogicalAndFilter(ISelectionFilter first, params ISelectionFilter[] filters) : base(first, filters) { }
			public LogicalAndFilter(IEnumerable<ISelectionFilter> filters) : base(filters) { }
			public override bool AllowElement(Element elem)
			{
				bool erg = true;
				if (ExecuteAll)
				{
					filters.ForEach(filter => erg &= filter.AllowElement(elem));
				}
				else
				{
					erg = filters.All(filter => filter.AllowElement(elem));
				}
				return erg;
			}
			public override bool AllowReference(Reference reference, XYZ position)
			{
				bool erg = true;
				if (ExecuteAll)
				{
					filters.ForEach(filter => erg &= filter.AllowReference(reference, position));
				}
				else
				{
					erg = filters.All(filter => filter.AllowReference(reference, position));
				}
				return erg;
			}
		}
		private class LogicalNotFilter : ISelectionFilter
		{
			private readonly ISelectionFilter filter;
			public LogicalNotFilter(ISelectionFilter filter)
			{
				this.filter = filter;
			}
			public bool AllowElement(Element elem)
			{
				return !filter.AllowElement(elem);
			}
			public bool AllowReference(Reference reference, XYZ position)
			{
				return !filter.AllowReference(reference, position);
			}
		}
		#endregion

		#endregion

		#region Extension methods for selection filters
		public static ILogicalCombinationFilter Or(this ISelectionFilter filter, params ISelectionFilter[] filters)
		{
			return new LogicalOrFilter(filter, filters);
		}
		public static ILogicalCombinationFilter And(this ISelectionFilter filter, params ISelectionFilter[] filters)
		{
			return new LogicalAndFilter(filter, filters);
		}
		public static ISelectionFilter Not(this ISelectionFilter filter)
		{
			return new LogicalNotFilter(filter);
		}
		#endregion

	}
}
