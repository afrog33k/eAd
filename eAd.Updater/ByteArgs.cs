using System;

namespace eAd.Updater
{
public class ByteArgs : EventArgs
{
    public int Downloaded
    {
        get;
        set;
    }

    public int total
    {
        get;
        set;
    }
}
}