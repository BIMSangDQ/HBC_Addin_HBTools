using System.Collections.ObjectModel;
using Autodesk.Revit.DB;
using Utils;

namespace HoaBinhTools.PurgeViewsV2.Models
{
	public class InfoLink : ViewModelBase
	{

		public InfoLink()
		{
			this.Links = new ObservableCollection<MyLink>();
		}

		public string LinkType { get; set; }

		public bool isCheckAll = false;

		public bool IsCheckAll
		{
			get
			{

				return isCheckAll;
			}
			set
			{
				this.isCheckAll = value;

				if (isCheckAll == true && Links != null)
				{

					for (int i = 0; i < Links.Count; i++)
					{
						Links[i].IsCheck = true;
					}


				}
				if (isCheckAll == false && Links != null)
				{

					for (int i = 0; i < Links.Count; i++)
					{
						Links[i].IsCheck = false;
					}

				}
				OnPropertyChanged(nameof(IsCheckAll));
			}
		}

		public ObservableCollection<MyLink> Links { get; set; }
	}

	public class MyLink : ViewModelBase
	{
		public string Name { get; set; }

		public ElementId Id;

		public bool isCheck;

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
	}
}
