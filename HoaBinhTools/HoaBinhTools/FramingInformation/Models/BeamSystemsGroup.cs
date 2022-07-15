using System.Collections.ObjectModel;
using System.Linq;
using Utils;

namespace HoaBinhTools.FramingInformation.Models
{
	public class BeamSystemsGroup : ViewModelBase
	{
		public ObservableCollection<MovableRestBeam> movable;

		public ObservableCollection<MovableRestBeam> Movable
		{
			get
			{
				return movable;
			}
			set
			{
				this.movable = value;

				OnPropertyChanged(nameof(Movable));
			}
		}


		public int SpanCount
		{
			get
			{
				return Movable.Where(e => e.Type == TypeAdjSupport.ND).Count();
			}
			set
			{
				OnPropertyChanged(nameof(SpanCount));
			}

		}

		public int SupportCount
		{
			get
			{
				return Movable.Where(e => e.Type == TypeAdjSupport.GT).Count();
			}
			set
			{
				OnPropertyChanged(nameof(SupportCount));
			}

		}

		public int SecondaryBeamCount
		{
			get
			{
				return Movable.Where(e => e.Type == TypeAdjSupport.DP).Count();
			}
			set
			{
				OnPropertyChanged(nameof(SecondaryBeamCount));
			}

		}

		public string name;
		public string Name
		{
			get => name;

			set
			{
				this.name = value;

				OnPropertyChanged(nameof(Name));
			}
		}


		public HostFraming host;
		public HostFraming Host
		{
			get => host;

			set
			{
				this.host = value;

				OnPropertyChanged(nameof(Host));
			}
		}

		public string groupID;
		public string GroupID
		{
			get => groupID;

			set
			{
				this.groupID = value;

				OnPropertyChanged(nameof(GroupID));
			}
		}
	}
}
