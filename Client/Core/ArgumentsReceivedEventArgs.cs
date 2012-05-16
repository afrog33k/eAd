using System;

namespace ClientApp.Core
{
public class ArgumentsReceivedEventArgs : EventArgs
{
    public string[] Args
    {
        get;
        set;
    }
}
}

