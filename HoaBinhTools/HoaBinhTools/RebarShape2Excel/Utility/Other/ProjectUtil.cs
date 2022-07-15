using SingleData;

namespace Utility
{
	public static partial class ProjectUtil
	{
		private static TemplateData templateData
		{
			get
			{
				return TemplateData.Instance;
			}
		}
		private static RevitData revitData
		{
			get
			{
				return RevitData.Instance;
			}
		}
		public static void Initial()
		{
			var projectInfo = revitData.Document.ProjectInformation;
			templateData.Project = new Model.Entity.Project { Name = projectInfo.Name, Code = projectInfo.Number };
		}
	}
}
