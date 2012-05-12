namespace DesktopClient
{
using System;
using System.Runtime.CompilerServices;

public class ArgumentsReceivedEventArgs : EventArgs
{
    public string[] Args
    {
        get;
        set;
    }
}
}

