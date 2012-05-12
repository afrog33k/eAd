using System;

namespace eAd.DataViewModels
{
/// <remarks/>
[System.SerializableAttribute()]

[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class RequiredFileModel
{
    // [System.Xml.Serialization.XmlAttributeAttribute()]
    /// <remarks/>

    public string FileType
    {
        get;
        set;
    }
    /// <remarks/>

    public string Path
    {
        get;
        set;
    }




    public bool Downloading
    {
        get;
        set;
    }

    public long ChunkOffset
    {
        get;
        set;
    }
    public long ChunkSize
    {
        get;
        set;
    }


    public int Retrys
    {
        get;
        set;
    }

    /// <remarks/>

    public long Id
    {
        get;
        set;
    }
    /// <remarks/>

    public long Size
    {
        get;
        set;
    }
    /// <remarks/>

    public string MD5
    {
        get;
        set;
    }

    public bool Complete
    {
        get;
        set;
    }

    public DateTime LastChecked
    {
        get;
        set;
    }

    public object Other
    {
        get;    // Any Other Data for now
        set;
    }
}
}