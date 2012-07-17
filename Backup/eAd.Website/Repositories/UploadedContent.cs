using System;
using System.Collections.Generic;
using eAd.Website.Controllers;

namespace eAd.Website.Repositories
{
public class UploadedContent
{
    public TimeSpan Duration
    {
        get;
        set;
    }
    public string MediaGuid
    {
        get;
        set;
    }
    public UploadType Type
    {
        get;
        set;
    }
    public List<string> Pictures
    {
        get;
        set;
    }
}
}