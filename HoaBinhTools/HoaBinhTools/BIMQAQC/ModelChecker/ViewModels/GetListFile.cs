using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HoaBinhTools.BIMQAQC.ModelChecker.ViewModels
{
	public class GetListFile
	{
		public static List<string> GetPathRevit(string RevitVersion)
		{
			string url = string.Format("https://script.google.com/macros/s/AKfycbxqJrKzeviqk6WJhRRhuu9gtVPsA5oyfgBigQoCiG3zAUdUzTQ/exec?Rvv={0}", RevitVersion);
			HttpWebRequest req;
			HttpWebResponse res = null;

			string user = "";
			try
			{
				req = (HttpWebRequest)WebRequest.Create(url);
				res = (HttpWebResponse)req.GetResponse();
				Stream stream = res.GetResponseStream();

				StreamReader readStream = new StreamReader(stream, Encoding.UTF8);

				user = readStream.ReadToEnd();

				res.Close();
				readStream.Close();
				List<string> Path = new List<string>();
				var p = user.Split(',');

				for (int i = 0; i < p.Length; i++)
				{
					p[i] = p[i].Replace("\0", "");
					p[i] = p[i].Replace("\"", "");
					Path.Add(p[i]);
				}
				return Path;
			}
			finally
			{
				if (res != null)
					res.Close();

			}
		}

		public static List<string> GetListAdmin()
		{
			string url = "https://script.google.com/macros/s/AKfycbwZUFsVvCT3m-XFBw1ijxx2YnwBilYkN60P-c1Dk9uJvcFP26o/exec?id=1";
			
			HttpWebRequest req;
			HttpWebResponse res = null;

			string user = "";
			try
			{
				req = (HttpWebRequest)WebRequest.Create(url);
				res = (HttpWebResponse)req.GetResponse();
				Stream stream = res.GetResponseStream();

				StreamReader readStream = new StreamReader(stream, Encoding.UTF8);

				user = readStream.ReadToEnd();

				res.Close();
				readStream.Close();
				List<string> Path = new List<string>();
				var p = user.Split(',');

				for (int i = 0; i < p.Length; i++)
				{
					Path.Add(p[i]);
				}
				return Path;
			}
			finally
			{
				if (res != null)
					res.Close();

			}
		}

		public static string GetDisciplineCheckName(string FileName)
		{
			string pattern = @"^(?<Project>(.+))-HBC_(?<Discipline>(A|S|M))?-";
			Regex reg = new Regex(pattern);

			string Project = "";
			string Discipline = "";
			foreach (Match result in reg.Matches(FileName))
			{
				Project = result.Groups["Project"].ToString();
				Discipline = result.Groups["Discipline"].ToString();
			}

			string url = $"https://script.google.com/macros/s/AKfycbwwlI8bNiNkDCpDa7oyMpmhXkOmnSycoCmrYYWp/exec?Project={Project}&Discipline={Discipline}";

			HttpWebRequest req;
			HttpWebResponse res = null;

			string user = "";
			try
			{
				req = (HttpWebRequest)WebRequest.Create(url);
				res = (HttpWebResponse)req.GetResponse();
				Stream stream = res.GetResponseStream();

				StreamReader readStream = new StreamReader(stream, Encoding.UTF8);

				user = readStream.ReadToEnd();

				res.Close();
				readStream.Close();

				return user;
			}
			finally
			{
				if (res != null)
					res.Close();

			}
		}

		public static void AddCheckSetName(string CheckSetName)
		{
			string url = string.Format("https://script.google.com/macros/s/AKfycbwz7tSKflcc7T-yH94WHi4Xm2AbDE_sLcSHs9BabL8HCphFowme/exec/exec?CheckSetName={0}", CheckSetName);
			HttpWebRequest req;
			HttpWebResponse res = null;

			string user = "";
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
			}
			finally
			{
				if (res != null)
					res.Close();
			}
		}

		public static void AddCheckSetDisciplineName(string CheckSetName)
		{
			string url = string.Format("https://script.google.com/macros/s/AKfycbyBR8YyvUxnlzw3Nh0sVw_SdBinTqEMDDBfDZMOPA/exec?CheckSetName={0}", CheckSetName);
			HttpWebRequest req;
			HttpWebResponse res = null;

			string user = "";
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
			}
			finally
			{
				if (res != null)
					res.Close();
			}
		}

		public static List<double> GetMandotoryCondition()
		{
			string url = string.Format("https://script.google.com/macros/s/AKfycbz9vuVwaL1J2_PH7X21-e0wD-MqU28dEGjjOc04sQ/exec");
			HttpWebRequest req;
			HttpWebResponse res = null;

			string user = "";
			try
			{
				req = (HttpWebRequest)WebRequest.Create(url);
				res = (HttpWebResponse)req.GetResponse();
				Stream stream = res.GetResponseStream();

				StreamReader readStream = new StreamReader(stream, Encoding.UTF8);

				user = readStream.ReadToEnd();

				res.Close();
				readStream.Close();
				List<double> Path = new List<double>();
				var p = user.Split(',');

				for (int i = 0; i < p.Length; i++)
				{
					p[i] = p[i].Replace("\0", "");
					p[i] = p[i].Replace("\"", "");
					Path.Add(double.Parse(p[i]));
				}
				return Path;
			}
			finally
			{
				if (res != null)
					res.Close();

			}
		}
		private static string Process(byte[] data, int read)
		{
			string v = (ASCIIEncoding.ASCII.GetString(data));
			return v;
		}
	}
}
