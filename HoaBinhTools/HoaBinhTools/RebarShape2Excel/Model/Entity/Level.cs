using Utility;

namespace Model.Entity
{
	public class Level
	{
		public string Name { get; set; }
		public virtual string HostLevelName { get; set; }
		private Level hostLevel;
		public virtual Level HostLevel
		{
			get
			{
				if (hostLevel == null)
				{
					hostLevel = this.GetHostLevel();
				}
				return hostLevel;
			}
		}
	}
}