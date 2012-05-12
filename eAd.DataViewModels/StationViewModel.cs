namespace eAd.DataViewModels
{
using System;
using System.Runtime.CompilerServices;

public class StationViewModel
{
    private bool _isOnline;

    public bool Available
    {
        get;
        set;
    }

    public bool IsOnline
    {
        get
        {
            return this._isOnline;
        }
        set
        {
            this._isOnline = value;
        }
    }

    public DateTime? LastCheckIn
    {
        get;
        set;
    }

    public string Location
    {
        get;
        set;
    }

    public string Name
    {
        get;
        set;
    }

    public long StationID
    {
        get;
        set;
    }

    public string Status
    {
        get;
        set;
    }
}
}

