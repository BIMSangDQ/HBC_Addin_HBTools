using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;

namespace Schedule2Excel2k16.Util
{
	class JsonConfigParser
	{
		public static Dictionary<String, String> readJson(String fileName)
		{
			JavaScriptSerializer json = new JavaScriptSerializer();
			Dictionary<String, String> data = new Dictionary<string, string>();

			if (!File.Exists(fileName))
			{
				if (!Directory.Exists(Path.GetDirectoryName(fileName)))
				{
					Directory.CreateDirectory(Path.GetDirectoryName(fileName));
				}

				data.Add("show_warning", "on");
				File.WriteAllText(fileName, json.Serialize(data));
				return data;
			}

			String strJson = File.ReadAllText(fileName);
			data = json.Deserialize<Dictionary<String, String>>(strJson);

			return data;

		}

		public static void saveJson(String fileName, Dictionary<String, String> data)
		{
			JavaScriptSerializer json = new JavaScriptSerializer();

			if (!Directory.Exists(Path.GetDirectoryName(fileName)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(fileName));
			}

			File.WriteAllText(fileName, json.Serialize(data));

		}
	}
}
