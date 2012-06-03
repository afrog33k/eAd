using System.Collections.Generic;

namespace eAd.DataViewModels
{
/// <remarks/>

[System.SerializableAttribute()]

[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class ScheduleModel
{
    /// <remarks/>
    public List<ScheduleLayout> Items
    {
        get;
        set;
    }
}
}
