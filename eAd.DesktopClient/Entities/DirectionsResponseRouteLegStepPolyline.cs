namespace DesktopClient.Entities
{
/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class DirectionsResponseRouteLegStepPolyline
{

    private string pointsField;

    private string levelsField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string points
    {
        get
        {
            return this.pointsField;
        }
        set
        {
            this.pointsField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string levels
    {
        get
        {
            return this.levelsField;
        }
        set
        {
            this.levelsField = value;
        }
    }
}
}