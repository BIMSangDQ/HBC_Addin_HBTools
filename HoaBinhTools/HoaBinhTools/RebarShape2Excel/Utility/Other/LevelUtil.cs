using System.Linq;
using SingleData;

namespace Utility
{
	public static class LevelUtil
	{
		private static TemplateData templateData;
		private static RevitData revitData;
		public static void Initial()
		{
			GetSingleData();

			var levels = templateData.Levels;
			foreach (var level in revitData.Levels)
			{
				levels.Add(
					new Model.Entity.Level
					{
						Name = level.Name,
						HostLevelName = level.LookupParameter("HB_TangChu")?.AsString()
					});
			}
		}
		public static Model.Entity.Level GetHostLevel(this Model.Entity.Level level)
		{
			GetSingleData();

			var hostLevel = templateData.Levels.SingleOrDefault(x => x.Name == level.HostLevelName);
			if (hostLevel == null) hostLevel = level;
			return hostLevel;
		}

		public static Model.Entity.Level GetLevel(this string name)
		{
			GetSingleData();

			return templateData.Levels.SingleOrDefault(x => x.Name == name);
		}
		private static void GetSingleData()
		{
			if (Singleton.Instance != null)
			{
				revitData = RevitData.Instance;
				templateData = TemplateData.Instance;
			}
		}
	}
}