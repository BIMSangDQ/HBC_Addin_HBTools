using System.Collections.Generic;
using HoaBinhTools.QLUser.Models;
using HoaBinhTools.QLUser.Views;
using Utils;

namespace HoaBinhTools.QLUser.ViewModels
{
	class QLUserViewModel : ViewModelBase
	{

		public RelayCommand btnSendFeedBack { get; set; }

		public RelayCommand btnRegistration { get; set; }

		public RelayCommand btnUpdate { get; set; }
		UserView Usermain { get; set; }

		public User user { get; set; }

		public void Execute()
		{
			SetUser setuser = new SetUser();
			user = setuser.GetInforUser();

			GetListAddin getListAddin = new GetListAddin();
			ListAddin = getListAddin.ListAddin();

			Usermain = new UserView(this);

			if (Usermain.ShowDialog() == true)
			{
				Run();
			}
		}


		public QLUserViewModel()
		{
			btnSendFeedBack = new RelayCommand(BtnSendFeedBack);

			btnRegistration = new RelayCommand(BtnRegistration);

			btnUpdate = new RelayCommand(BtnUpdate);
		}

		public void BtnSendFeedBack()
		{
			GetListAddin getListAddin = new GetListAddin();
			getListAddin.SendFeedback(fullName, addinName, feedBack);
		}

		public void BtnRegistration()
		{
			Usermain.DialogResult = true;
			Usermain.Close();
		}

		public void BtnUpdate()
		{
			Registration reg = new Registration();
			reg.UpdateInfor(hdd, computerUser, employerNumber, fullName, mail, phoneNumber, ctr);
			Usermain.Close();
		}

		public void Run()
		{
			Registration reg = new Registration();
			reg.RegistrationAddin(hdd, computerUser, employerNumber, fullName, mail, phoneNumber, ctr);
		}

		public List<string> ListAddin { get; set; }

		private string hdd = "";
		public string HDD
		{
			get
			{
				return hdd = user.Serial;
			}
			set
			{
				this.hdd = value;
			}

		}

		private string computerUser = "";
		public string ComputerUser
		{
			get
			{
				return computerUser = user.UserCpt;
			}
			set
			{
				this.computerUser = value;
			}
		}

		private string employerNumber = "";
		public string EmployerNumber
		{
			get
			{
				return employerNumber = user.EmpolyerNumber;
			}
			set
			{
				this.employerNumber = value;
				this.user.EmpolyerNumber = value;
			}
		}

		private string fullName = "";
		public string FullName
		{
			get
			{
				return fullName = user.FullName;
			}
			set
			{
				this.fullName = value;
				this.user.FullName = value;

			}
		}

		private string mail = "";
		public string Mail
		{
			get
			{
				return mail = user.Mail;
			}
			set
			{
				this.mail = value;
				this.user.Mail = value;
			}
		}

		private string phoneNumber = "";
		public string PhoneNumber
		{
			get
			{
				return phoneNumber = user.PhoneNumber;
			}
			set
			{
				this.phoneNumber = value;
				this.user.PhoneNumber = value;
			}
		}

		private string ctr = "";
		public string Ctr
		{
			get
			{
				return ctr = user.Ctr;
			}
			set
			{
				this.ctr = value;
				this.user.Ctr = value;
			}
		}

		private string feedBack = "";
		public string FeedBack
		{
			get
			{
				return feedBack;
			}
			set
			{
				this.feedBack = value;
			}
		}

		private string addinName = "";
		public string AddinName
		{
			get
			{
				return addinName;
			}
			set
			{
				this.addinName = value;
			}
		}


	}
}
