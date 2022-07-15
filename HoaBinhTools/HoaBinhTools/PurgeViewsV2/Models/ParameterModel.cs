using Autodesk.Revit.DB;
using Utils;

namespace HoaBinhTools.PurgeViewsV2.Models
{
	public class ParameterModel : ViewModelBase
	{

		public string Name { get; set; }


		public ElementId Id { get; set; }

		private bool isCheck;

		public bool IsCheck
		{

			get
			{
				return isCheck;
			}
			set
			{
				isCheck = value;

				OnPropertyChanged(nameof(IsCheck));
			}
		}

		public ParameterModel(DefinitionBindingMapIterator it)
		{
			Name = it.Key.Name;

			Id = ((InternalDefinition)it.Key).Id;

			IsCheck = false;

		}

	}
}
