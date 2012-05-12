namespace eAd.DataViewModels
{
using System;
using System.Runtime.CompilerServices;

public class MessageViewModel
{
    public string Command
    {
        get;
        set;
    }

    public long ID
    {
        get;
        set;
    }

    public bool Sent
    {
        get;
        set;
    }

    public long StationID
    {
        get;
        set;
    }

    public string Text
    {
        get;
        set;
    }

    public string Type
    {
        get;
        set;
    }

    public long UserID
    {
        get;
        set;
    }
}
}

