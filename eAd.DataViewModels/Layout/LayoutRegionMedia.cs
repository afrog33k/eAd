using System.Collections.Generic;

namespace eAd.DataViewModels
{
/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public  class LayoutRegionMedia
{
    /// <remarks/>
    //      [System.Xml.Serialization.XmlElementAttribute("options", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public LayoutRegionMediaOptions Options
    {
        get;
        set;
    }

    /// <remarks/>
    // [System.Xml.Serialization.XmlElementAttribute("raw", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public LayoutRegionMediaRaw Raw
    {
        get;
        set;
    }

    /// <remarks/>
    //  [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Id
    {
        get;
        set;
    }

    /// <remarks/>
    //   [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Type
    {
        get;
        set;
    }

    /// <remarks/>
    //    [System.Xml.Serialization.XmlAttributeAttribute()]
    public int Duration
    {
        get;
        set;
    }

    /// <remarks/>
    //  [System.Xml.Serialization.XmlAttributeAttribute()]
    public int Lkid
    {
        get;
        set;
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public int UserId
    {
        get;
        set;
    }

    /// <remarks/>
    //   [System.Xml.Serialization.XmlAttributeAttribute()]
    public int SchemaVersion
    {
        get;
        set;
    }

    public override string ToString()
    {
        return Type + Id;
    }
}
}