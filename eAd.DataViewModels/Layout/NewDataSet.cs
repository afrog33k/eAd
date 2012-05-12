namespace eAd.DataViewModels
{
/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public  class NewDataSet
{

    private LayoutModel[] _itemsField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("layout")]
    public LayoutModel[] Items
    {
        get
        {
            return this._itemsField;
        }
        set
        {
            this._itemsField = value;
        }
    }
}
}