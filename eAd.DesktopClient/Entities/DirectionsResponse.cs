namespace DesktopClient.Entities
{
/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class DirectionsResponse
{

    private string statusField;

    private DirectionsResponseRoute[] routeField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string status
    {
        get
        {
            return this.statusField;
        }
        set
        {
            this.statusField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("route", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public DirectionsResponseRoute[] route
    {
        get
        {
            return this.routeField;
        }
        set
        {
            this.routeField = value;
        }
    }
}
}