using System;

namespace Client.Update
{
public class VersionInfo
{
    public int Major
    {
        get;
        set;
    }
    public int Minor
    {
        get;
        set;
    }


    public VersionInfo()
    { }

    public VersionInfo(int major, int minor )
    {
        Major = major;
        Minor = minor;

    }
    public override string ToString()
    {
        return String.Format("{0}.{1}", Major, Minor);
    }
}
}