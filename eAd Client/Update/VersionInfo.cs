namespace ClientApp.Update
{
    using System;
    using System.Runtime.CompilerServices;

    public class VersionInfo
    {
        public VersionInfo()
        {
        }

        public VersionInfo(int major, int minor)
        {
            this.Major = major;
            this.Minor = minor;
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}", this.Major, this.Minor);
        }

        public int Major { get; set; }

        public int Minor { get; set; }
    }
}

