namespace ClientApp
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct LayoutSchedule
    {
        public string NodeName;
        public string LayoutFile;
        public int ID;
        public int Scheduleid;
        public int Priority;
        public DateTime FromDate;
        public DateTime ToDate;
    }
}

