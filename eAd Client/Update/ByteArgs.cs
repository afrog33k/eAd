namespace ClientApp.Update
{
    using System;
    using System.Runtime.CompilerServices;

    public class ByteArgs : EventArgs
    {
        public int Downloaded { get; set; }

        public int Total { get; set; }
    }
}

