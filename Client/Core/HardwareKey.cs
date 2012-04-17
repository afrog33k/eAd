using System;
using System.Management;
using Client.Properties;
using eAd.Utilities;

namespace Client.Core
{
    class HardwareKey
    {
        private string _hardwareKey;
        private string _macAddress;

        public string MacAddress
        {
            get
            {
                return _macAddress;
            }
        }

        public HardwareKey()
        {
            System.Diagnostics.Debug.WriteLine("[IN]", "HardwareKey");

            // Get the key from the Settings
            _hardwareKey = Settings.Default.hardwareKey;

            // Is the key empty?
            if (_hardwareKey == "")
            {
                try
                {
                    // Calculate the Hardware key from the CPUID and Volume Serial
                    _hardwareKey = Hashes.MD5(GetCPUId() + GetVolumeSerial("C"));
                }
                catch
                {
                    _hardwareKey = "Change for Unique Key";
                }

                // Store the key
                Settings.Default.hardwareKey = _hardwareKey;
            }

            // Get the Mac Address
            _macAddress = GetMACAddress();

            System.Diagnostics.Debug.WriteLine("[OUT]", "HardwareKey");
        }

        /// <summary>
        /// Gets the hardware key
        /// </summary>
        public string Key
        {
            get 
            { 
                return this._hardwareKey; 
            }
        }

        /// <summary>
        /// Regenerates the hardware key
        /// </summary>
        public void Regenerate()
        {
            // Calculate the Hardware key from the CPUID and Volume Serial
            _hardwareKey = Hashes.MD5(GetCPUId() + GetVolumeSerial("C"));

            // Store the key
            Settings.Default.hardwareKey = _hardwareKey;
            Settings.Default.Save();
        }

        /// <summary>
        /// return Volume Serial Number from hard drive
        /// </summary>
        /// <param name="strDriveLetter">[optional] Drive letter</param>
        /// <returns>[string] VolumeSerialNumber</returns>
        public string GetVolumeSerial(string strDriveLetter)
        {
            System.Diagnostics.Debug.WriteLine("[IN]", "GetVolumeSerial");

            if (strDriveLetter == "" || strDriveLetter == null) strDriveLetter = "C";
            ManagementObject disk = new ManagementObject("win32_logicaldisk.deviceid=\"" + strDriveLetter + ":\"");
            disk.Get();

            System.Diagnostics.Debug.WriteLine("[OUT]", "GetVolumeSerial");

            return disk["VolumeSerialNumber"].ToString();
        }

        /// <summary>
        /// Returns MAC Address from first Network Card in Computer
        /// </summary>
        /// <returns>[string] MAC Address</returns>
        public string GetMACAddress()
        {
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            string MACAddress = String.Empty;

            foreach (ManagementObject mo in moc)
            {
                if (MACAddress == String.Empty)  // only return MAC Address from first card
                {
                    if ((bool)mo["IPEnabled"] == true) MACAddress = mo["MacAddress"].ToString();
                }
                mo.Dispose();
            }
            
            return MACAddress;
        }

        /// <summary>
        /// Return processorId from first CPU in machine
        /// </summary>
        /// <returns>[string] ProcessorId</returns>
        public string GetCPUId()
        {
            System.Diagnostics.Debug.WriteLine("[IN]", "GetCPUId");

            string cpuInfo = String.Empty;
            string temp = String.Empty;
            ManagementClass mc = new ManagementClass("Win32_Processor");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (cpuInfo == String.Empty)
                {   // only return cpuInfo from first CPU
                    cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                }
            }

            System.Diagnostics.Debug.WriteLine("[OUT]", "GetCPUId");

            return cpuInfo;
        }
    }
}