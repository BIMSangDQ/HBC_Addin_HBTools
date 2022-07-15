using HoaBinhTools.FramingInformation.Views;
using Utils;
using static HoaBinhTools.FramingInformation.Models.EnumSetting;

namespace HoaBinhTools.FramingInformation.ViewModels
{
	public class SettingGeoInfoRevitViewModels : ViewModelBase
	{
		public SettingGeoInfoRevitViewModels()
		{
			SettingGeoInfoRevit MainForm = new SettingGeoInfoRevit(this);

			MainForm.ShowDialog();
		}

		ViewSetting choiceView = (ViewSetting)HoaBinhTools.Properties.Settings.Default.ViewSetting;

		public ViewSetting NoteView
		{
			get { return this.choiceView; }

			set
			{
				if (this.choiceView == value)
				{
					return;
				}
				this.choiceView = value;

				OnPropertyChanged(nameof(NoteView));

			}
		}

		public bool IsProject
		{
			get { return NoteView == ViewSetting.project; }

			set
			{
				NoteView = value ? ViewSetting.project : NoteView;

				HoaBinhTools.Properties.Settings.Default.ViewSetting = (int)NoteView;

				Properties.Settings.Default.Save();

				OnPropertyChanged(nameof(IsProject));
			}
		}

		public bool IsActiveView
		{

			get { return NoteView == ViewSetting.activeView; }

			set
			{
				NoteView = value ? ViewSetting.activeView : NoteView;

				HoaBinhTools.Properties.Settings.Default.ViewSetting = (int)NoteView;

				Properties.Settings.Default.Save();

				OnPropertyChanged(nameof(IsActiveView));
			}
		}



		public bool isSelect = HoaBinhTools.Properties.Settings.Default.SelectObject;
		public bool IsSelect
		{
			get { return isSelect; }

			set
			{
				this.isSelect = value;


				HoaBinhTools.Properties.Settings.Default.SelectObject = value;

				Properties.Settings.Default.Save();

				OnPropertyChanged(nameof(IsSelect));
			}
		}


		public bool isPick = HoaBinhTools.Properties.Settings.Default.PickObject;
		public bool IsPick
		{
			get { return isPick; }

			set
			{
				this.isPick = value;

				HoaBinhTools.Properties.Settings.Default.PickObject = isPick;

				Properties.Settings.Default.Save();

				OnPropertyChanged(nameof(isPick));

			}
		}


		public bool maxLength = HoaBinhTools.Properties.Settings.Default.MaxLength;
		public bool MaxLength
		{
			get { return maxLength; }

			set
			{
				this.maxLength = value;

				HoaBinhTools.Properties.Settings.Default.MaxLength = maxLength;

				Properties.Settings.Default.Save();

				OnPropertyChanged(nameof(maxLength));

			}
		}




		public double giatCap = HoaBinhTools.Properties.Settings.Default.GiatCap;
		public double GiatCap
		{
			get { return giatCap; }

			set
			{
				this.giatCap = value;

				HoaBinhTools.Properties.Settings.Default.GiatCap = giatCap;

				Properties.Settings.Default.Save();

				OnPropertyChanged(nameof(giatCap));
			}
		}

		public double gocLechGiahaiDam = HoaBinhTools.Properties.Settings.Default.GocLechGiahaiDam;
		public double GocLechGiahaiDam
		{
			get { return gocLechGiahaiDam; }

			set
			{
				this.gocLechGiahaiDam = value;

				HoaBinhTools.Properties.Settings.Default.GocLechGiahaiDam = gocLechGiahaiDam;

				Properties.Settings.Default.Save();

				OnPropertyChanged(nameof(GocLechGiahaiDam));

			}
		}



		public double khoangCach = HoaBinhTools.Properties.Settings.Default.KhoangCach;
		public double KhoangCach
		{
			get { return khoangCach; }

			set
			{
				this.khoangCach = value;

				HoaBinhTools.Properties.Settings.Default.KhoangCach = khoangCach;

				Properties.Settings.Default.Save();

				OnPropertyChanged(nameof(KhoangCach));

			}
		}


		public double saiSoMatCat = HoaBinhTools.Properties.Settings.Default.ToleranceSection;
		public double SaiSoMatCat
		{
			get { return saiSoMatCat; }

			set
			{
				this.saiSoMatCat = value;

				HoaBinhTools.Properties.Settings.Default.ToleranceSection = saiSoMatCat;

				Properties.Settings.Default.Save();

				OnPropertyChanged(nameof(SaiSoMatCat));

			}
		}



		public bool isSecondaryBeam = HoaBinhTools.Properties.Settings.Default.IsSecondaryBeam;
		public bool IsSecondaryBeam
		{
			get
			{
				return isSecondaryBeam;
			}
			set
			{
				this.isSecondaryBeam = value;

				HoaBinhTools.Properties.Settings.Default.IsSecondaryBeam = isSecondaryBeam;

				Properties.Settings.Default.Save();

				OnPropertyChanged(nameof(IsSecondaryBeam));
			}
		}






		public string faramingHostName = HoaBinhTools.Properties.Settings.Default.NameHost;
		public string FaramingHostName
		{
			get
			{
				return faramingHostName;
			}
			set
			{
				this.faramingHostName = value;

				HoaBinhTools.Properties.Settings.Default.NameHost = faramingHostName;

				Properties.Settings.Default.Save();
			}
		}



		Direction choiceDirectionVertical = (Direction)HoaBinhTools.Properties.Settings.Default.Vertical;
		public Direction NoteDirectionVertical
		{
			get { return this.choiceDirectionVertical; }

			set
			{
				if (this.choiceDirectionVertical == value)
				{
					return;
				}
				this.choiceDirectionVertical = value;

				OnPropertyChanged(nameof(NoteDirectionVertical));
			}
		}


		public bool IsUP
		{
			get { return NoteDirectionVertical == Direction.Up; }

			set
			{
				NoteDirectionVertical = value ? Direction.Up : NoteDirectionVertical;

				HoaBinhTools.Properties.Settings.Default.Vertical = (int)NoteDirectionVertical;

				Properties.Settings.Default.Save();

				OnPropertyChanged(nameof(IsUP));
			}
		}



		public bool IsDown
		{
			get { return NoteDirectionVertical == Direction.Down; }

			set
			{
				NoteDirectionVertical = value ? Direction.Down : NoteDirectionVertical;

				HoaBinhTools.Properties.Settings.Default.Vertical = (int)NoteDirectionVertical;

				Properties.Settings.Default.Save();

				OnPropertyChanged(nameof(IsDown));
			}
		}


		Direction choiceDirectionHorizal = (Direction)HoaBinhTools.Properties.Settings.Default.Horizonl;

		public Direction NoteDirectionHorizal
		{
			get { return this.choiceDirectionHorizal; }

			set
			{
				if (this.choiceDirectionHorizal == value)
				{
					return;
				}
				this.choiceDirectionHorizal = value;

				OnPropertyChanged(nameof(NoteDirectionHorizal));
			}
		}

		public bool IsRight
		{

			get { return NoteDirectionHorizal == Direction.Right; }

			set
			{
				NoteDirectionHorizal = value ? Direction.Right : NoteDirectionHorizal;

				HoaBinhTools.Properties.Settings.Default.Horizonl = (int)NoteDirectionHorizal;

				Properties.Settings.Default.Save();

				OnPropertyChanged(nameof(IsRight));
			}
		}

		public bool IsLeft
		{
			get { return NoteDirectionHorizal == Direction.Left; }

			set
			{
				NoteDirectionHorizal = value ? Direction.Left : NoteDirectionHorizal;

				HoaBinhTools.Properties.Settings.Default.Horizonl = (int)NoteDirectionHorizal;

				Properties.Settings.Default.Save();

				OnPropertyChanged(nameof(IsLeft));
			}
		}
	}
}
