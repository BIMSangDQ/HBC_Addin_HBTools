using System.IO;
using Newtonsoft.Json;



namespace Utils
{
	public class JsonUtils
	{


		public static TypeGenerics GetSetting<TypeGenerics>(string filePath)
		{
			TypeGenerics obj = default;

			if (File.Exists(filePath))
			{
				try
				{
					using (StreamReader streamReader = File.OpenText(filePath))

						obj = (TypeGenerics)new JsonSerializer().Deserialize(streamReader, typeof(TypeGenerics));
				}
				catch
				{

				}

			}
			return obj;
		}






		//https://viblo.asia/p/su-dung-generics-trong-c-924lJDvNKPM


		public void SaveSetting<TypeGenerics>(TypeGenerics setting, string filePath)
		{
			string contents = JsonConvert.SerializeObject(setting, Newtonsoft.Json.Formatting.Indented);
			if (Directory.Exists(filePath) == false)
			{
				var s = Path.GetDirectoryName(filePath);
				Directory.CreateDirectory(s);
			}
			File.WriteAllText(filePath, contents);
		}


	}
}
