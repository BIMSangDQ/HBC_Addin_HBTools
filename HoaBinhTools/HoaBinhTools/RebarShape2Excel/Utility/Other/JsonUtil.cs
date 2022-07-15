using System.Web.Script.Serialization;
using SingleData;

namespace Utility
{
	public static class JsonUtil
	{
		private static RevitModelData revitModelData = RevitModelData.Instance;
		public static object ReadJson(string fileName)
		{
			var js = new JavaScriptSerializer();
			return js.DeserializeObject(fileName);
		}
	}
}
