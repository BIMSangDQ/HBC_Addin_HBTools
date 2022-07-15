using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Autodesk.Revit.UI;

namespace HoaBinhTools.QLUser.Models
{
	public class GetListAddin
	{
		public List<string> ListAddin()

		{
			string url = "https://script.google.com/macros/s/AKfycbz3gJiZnPG-NOJ57qjGs0HzPyuVXhRg_tOSOAvZkIN6pZDfMIpS/exec";
			HttpWebRequest req;
			HttpWebResponse res = null;
			string addin = "";

			try
			{
				req = (HttpWebRequest)WebRequest.Create(url);
				res = (HttpWebResponse)req.GetResponse();
				Stream stream = res.GetResponseStream();

				byte[] data = new byte[4096];
				int read;
				while ((read = stream.Read(data, 0, data.Length)) > 0)
				{
					addin = Process(data, read);
				}
				List<string> Addin = new List<string>();
				var v = addin.Split('|');
				for (int i = 0; i < v.Length; i++)
				{
					v[i] = v[i].Replace("\0", "");
					if (v[i] != null && v[i] != "")
					{
						Addin.Add(v[i]);
					}
				}
				res.Close();
				return Addin;

			}
			finally
			{
				if (res != null)
					res.Close();
			}
		}

		public void SendFeedback(string name, string addin, string cmt)
		{
			string url = string.Format("https://script.google.com/macros/s/AKfycbwx2J_OaYpIB6rv4XPK7UZ20rLx8W-Kr8gl-E1RjRFBCSmVpnA/exec?addin={0}&name={1}&content={2}", addin, name, cmt);
			HttpWebRequest req;
			HttpWebResponse res = null;
			try
			{
				req = (HttpWebRequest)WebRequest.Create(url);
				res = (HttpWebResponse)req.GetResponse();
				Stream stream = res.GetResponseStream();
				res.Close();

				TaskDialog.Show("BIM_HBC", "Chúng tôi đã nhận được phản hồi này và sẽ cập nhật trong lần gần nhất nếu có thể");

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
