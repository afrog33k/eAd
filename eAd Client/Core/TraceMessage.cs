namespace ClientApp.Core
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    internal struct TraceMessage
    {
        public string Message;
        public string DateTime;
        public string Category;
    }
}

