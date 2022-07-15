using System.Linq;
using System.Net.NetworkInformation;

namespace SingleData
{
	public class SecurityData
	{
		public static SecurityData Instance
		{
			get
			{
				return Singleton.Instance.SecurityData;
			}
		}

		private PhysicalAddress physicalAddress;
		public PhysicalAddress PhysicalAddress
		{
			get
			{
				if (physicalAddress == null)
				{
					physicalAddress = NetworkInterface.GetAllNetworkInterfaces()
						.Where(nic => nic.OperationalStatus == OperationalStatus.Up)
						.Select(nic => nic.GetPhysicalAddress()).First();
				}
				return physicalAddress;
			}
		}
	}
}
