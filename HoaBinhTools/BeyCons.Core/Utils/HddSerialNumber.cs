using System.Management;

namespace BeyCons.Core.Utils
{
	public class HddSerialNumber
	{
        public string GetHDDSerialNumber(string drive)
        {
            if (drive == "" || drive == null)
            {
                drive = "C";
            }
            ManagementObject disk = new ManagementObject("win32_logicaldisk.deviceid=\"" + drive + ":\"");
            disk.Get();
            return disk["VolumeSerialNumber"].ToString();
        }

        public string ComputerName()
        {
            return System.Windows.Forms.SystemInformation.ComputerName;
        }
    }
}
