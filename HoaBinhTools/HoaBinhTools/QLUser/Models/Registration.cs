using System.IO;
using System.Net;
using System.Text;
using Autodesk.Revit.UI;

namespace HoaBinhTools.QLUser.Models
{
	public class Registration
	{
		public void RegistrationAddin(string id, string username, string msnv, string name, string mail, string phonenumber, string ctr)
		{
			string url = string.Format("https://script.google.com/macros/s/AKfycbx12CE6eY-qSIyuAYfu1Clv0ux_VZHTPEPt4RwLCPZlmyWrnaMZ/exec?id={0}&username={1}&MSNV={2}&name={3}&mail={4}&sdt={5}&ctr={6}",
				id, username, msnv, name, mail, phonenumber, ctr);
			url = url.Replace(" ", "%20");
			HttpWebRequest req;
			HttpWebResponse res = null;
			string resp = "";

			try
			{
				req = (HttpWebRequest)WebRequest.Create(url);
				res = (HttpWebResponse)req.GetResponse();
				Stream stream = res.GetResponseStream();

				byte[] data = new byte[4096];
				int read;
				while ((read = stream.Read(data, 0, data.Length)) > 0)
				{
					resp = Process(data, read);
				}

				if (resp == "True")
				{
					TaskDialog.Show("BIM_HBC", "Đăng ký thành công vui lòng chờ kích hoạt và khởi động lại Revit!");
				}
				else
				{
					TaskDialog.Show("BIM_HBC", "Máy tính đã được đăng ký, vui lòng liên hệ để được kích hoạt.");
				}
			}
			finally
			{
				if (res != null)
					res.Close();
			}
		}

		public void UpdateInfor(string id, string username, string msnv, string name, string mail, string phonenumber, string ctr)
		{
			string url = string.Format("https://script.google.com/macros/s/AKfycbwsMAo7iqYvhsKjOopkc3B_8BBGC98UhI52irloElvimNNzZXK3/exec?id={0}&username={1}&MSNV={2}&name={3}&mail={4}&sdt={5}&ctr={6}",
				id, username, msnv, name, mail, phonenumber, ctr);
			url = url.Replace(" ", "%20");
			HttpWebRequest req;
			HttpWebResponse res = null;
			string resp = "";

			try
			{
				req = (HttpWebRequest)WebRequest.Create(url);
				res = (HttpWebResponse)req.GetResponse();
				TaskDialog.Show("BIM_HBC", "Thông tin người dùng đã được cập nhật!");
			}
			finally
			{
				if (res != null)
					res.Close();
			}
		}

		public void GhiNhanTanSuat(string name)
		{
			string id = SaveUser.Default.Hdd;
			HddSerialNumber hddSerialNumber = new HddSerialNumber();
			string computerName = hddSerialNumber.ComputerName();
			string url = string.Format("https://script.google.com/macros/s/AKfycbzmoERv3T30OkMwcyQcVrcHP8Omt21BOocJTIpRVlsJE3vGur9k/exec?id={0}&name={1}&computername={2}",
				id, name, computerName);
			url = url.Replace(" ", "%20");
			HttpWebRequest req;
			HttpWebResponse res = null;
			string resp = "";

			try
			{
				req = (HttpWebRequest)WebRequest.Create(url);
				res = (HttpWebResponse)req.GetResponse();
				res.Close();
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
}
