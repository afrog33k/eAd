using System;

namespace Client.Update
{
public class ByteArgs : EventArgs
{
    public int Downloaded
    {
        get;
        set;
    }

    public int Total
    {
        get;
        set;
    }
}
}