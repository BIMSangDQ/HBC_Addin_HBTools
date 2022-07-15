using SingleData;

namespace Utility
{
	public static partial class NavigationUtil
	{
		private static TemplateData templateData
		{
			get
			{

				return TemplateData.Instance;
			}
		}

		public static Model.Entity.Navigation AddNavigation(this Model.Entity.Identify identify)
		{
			var nav = new Model.Entity.Navigation { Identify = identify };
			templateData.Navigations.Add(nav);
			return nav;
		}
	}
}
