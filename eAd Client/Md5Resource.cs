namespace ClientApp
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct Md5Resource
    {
        public string MD5;
        public string Path;
        public DateTime CacheDate;
    }
}

