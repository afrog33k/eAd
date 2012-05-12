using System;

namespace Client
{
/// <summary>
/// A LayoutSchedule
/// </summary>
[Serializable]
public struct LayoutSchedule
{
    public string NodeName;
    public string LayoutFile;
    public int ID;
    public int Scheduleid;

    public int Priority;

    public DateTime FromDate;
    public DateTime ToDate;
}
}