using System.Collections.ObjectModel;
using Autodesk.Revit.DB;
using Utils;

namespace HoaBinhTools.PurgeViewsV2.Models
{
	public class InfoView : ViewModelBase
	{

		public InfoView()
		{
			this.Views = new ObservableCollection<MyView>();
		}

		public string ViewType { get; set; }

		private bool isCheckAll = false;

		public bool IsCheckAll
		{
			get
			{
				return isCheckAll;
			}
			set
			{
				this.isCheckAll = value;

				if (isCheckAll == true && Views != null)
				{
					for (int i = 0; i < Views.Count; i++)
					{
						Views[i].IsCheck = true;
					}

				}
				if (isCheckAll == false && Views != null)
				{
					for (int i = 0; i < Views.Count; i++)
					{
						Views[i].IsCheck = false;
					}
				}



				OnPropertyChanged(nameof(IsCheckAll));
			}

		}



		private bool isAll = false;

		public bool IsAll
		{
			get
			{

				return isAll;
			}
			set
			{
				this.isAll = value;

				if (isAll == true && Views != null)
				{

					for (int i = 0; i < Models.Count; i++)
					{
						Models[i].IsCheckAll = true;
					}


				}
				if (isAll == false && Views != null)
				{

					for (int i = 0; i < Models.Count; i++)
					{
						Models[i].IsCheckAll = false;
					}
				}

				OnPropertyChanged(nameof(IsAll));
			}
		}





		public ObservableCollection<MyView> Views { get; set; }



		private ObservableCollection<InfoView> models = null;
		public ObservableCollection<InfoView> Models
		{
			get
			{
				return models;
			}
			set
			{
				models = value;

				OnPropertyChanged();
			}
		}

	}

	public class MyView : ViewModelBase
	{

		public View view { get; set; }

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


		public MyView(View view)
		{
			isCheck = false;

			this.view = view;


		}
	}
}
