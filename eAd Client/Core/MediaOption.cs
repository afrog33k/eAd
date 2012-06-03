namespace ClientApp.Core
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct MediaOption
    {
        public string Name;
        public string Value;
    }
}

