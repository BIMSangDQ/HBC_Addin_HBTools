using System.IO;
using System.Net;
using System.Text;

namespace HoaBinhTools.QLUser.Models
{
	public class SetUser
	{
		public User GetInforUser()
		{
			HddSerialNumber hdd = new HddSerialNumber();
			string hddSerial = hdd.GetHDDSerialNumber("");
			string ComputerUser = hdd.ComputerName();
			string user = "";
			string url = string.Format("https://script.google.com/macros/s/AKfycbwdzauQmz13Al8gG-E63-TrcbheCc1sjEqaHzjUjFfDAfGsFkA/exec?id={0}&username={1}", hddSerial, ComputerUser);
			HttpWebRequest req;
			HttpWebResponse res = null;

			try
			{
				req = (HttpWebRequest)WebRequest.Create(url);
				res = (HttpWebResponse)req.GetResponse();
				Stream stream = res.GetResponseStream();

				byte[] data = new byte[4096];
				int read;
				while ((read = stream.Read(data, 0, data.Length)) > 0)
				{
					user = Process(data, read);
				}

				res.Close();
				User u = new User();

				u.Serial = hddSerial;
				u.UserCpt = ComputerUser;

				var a = user.Split('|');
				if (a.Length == 5)
				{

					u.EmpolyerNumber = a[0];
					u.FullName = a[1];
					u.Mail = a[2];
					u.PhoneNumber = a[3];
					u.Ctr = a[4].Replace("\0", "");
				}
				return u;
			}
			finally
			{
				if (res != null)
					res.Close();
			}
		}

		private string Process(byte[] data, int read)
		{
			string v = (ASCIIEncoding.ASCII.GetString(data));
			return v;
		}
	}
	public class User
	{
		public string Serial { get; set; }
		public string UserCpt { get; set; }
		public string EmpolyerNumber { get; set; }
		public string FullName { get; set; }
		public string Mail { get; set; }
		public string PhoneNumber { get; set; }
		public string Ctr { get; set; }
	}
}
