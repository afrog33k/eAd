using System;

namespace Client.Core
{
    public class ArgumentsReceivedEventArgs : EventArgs
    {
        public string[] Args { get; set; }
    }
}

