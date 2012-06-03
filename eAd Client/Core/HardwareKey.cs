namespace ClientApp.Core
{
    using ClientApp.Properties;
    using eAd.Utilities;
    using System;
    using System.Management;

    internal class HardwareKey
    {
        private string _hardwareKey = Settings.Default.hardwareKey;
        private string _macAddress;

        public HardwareKey()
        {
            if (this._hardwareKey == "")
            {
                try
                {
                    this._hardwareKey = Hashes.MD5(this.GetCPUId() + this.GetVolumeSerial("C"));
                }
                catch
                {
                    this._hardwareKey = "Change for Unique Key";
                }
                Settings.Default.hardwareKey = this._hardwareKey;
            }
            this._macAddress = this.GetMACAddress();
        }

        public string GetCPUId()
        {
            string str = string.Empty;
            ManagementClass class2 = new ManagementClass("Win32_Processor");
            foreach (ManagementObject obj2 in class2.GetInstances())
            {
                if (str == string.Empty)
                {
                    str = obj2.Properties["ProcessorId"].Value.ToString();
                }
            }
            return str;
        }

        public string GetMACAddress()
        {
            ManagementObjectCollection instances = new ManagementClass("Win32_NetworkAdapterConfiguration").GetInstances();
            string str = string.Empty;
            foreach (ManagementObject obj2 in instances)
            {
                if ((str == string.Empty) && ((bool) obj2["IPEnabled"]))
                {
                    str = obj2["MacAddress"].ToString();
                }
                obj2.Dispose();
            }
            return str;
        }

        public string GetVolumeSerial(string strDriveLetter)
        {
            if ((strDriveLetter == "") || (strDriveLetter == null))
            {
                strDriveLetter = "C";
            }
            ManagementObject obj2 = new ManagementObject("win32_logicaldisk.deviceid=\"" + strDriveLetter + ":\"");
            obj2.Get();
            return obj2["VolumeSerialNumber"].ToString();
        }

        public void Regenerate()
        {
            this._hardwareKey = Hashes.MD5(this.GetCPUId() + this.GetVolumeSerial("C"));
            Settings.Default.hardwareKey = this._hardwareKey;
            Settings.Default.Save();
        }

        public string Key
        {
            get
            {
                return this._hardwareKey;
            }
        }

        public string MacAddress
        {
            get
            {
                return this._macAddress;
            }
        }
    }
}

