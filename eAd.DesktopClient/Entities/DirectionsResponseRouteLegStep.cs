namespace DesktopClient.Entities
{
/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class DirectionsResponseRouteLegStep
{

    private string travel_modeField;

    private string html_instructionsField;

    private start_location[] start_locationField;

    private end_location[] end_locationField;

    private DirectionsResponseRouteLegStepPolyline[] polylineField;

    private duration[] durationField;

    private distance[] distanceField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string travel_mode
    {
        get
        {
            return this.travel_modeField;
        }
        set
        {
            this.travel_modeField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string html_instructions
    {
        get
        {
            return this.html_instructionsField;
        }
        set
        {
            this.html_instructionsField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("start_location")]
    public start_location[] start_location
    {
        get
        {
            return this.start_locationField;
        }
        set
        {
            this.start_locationField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("end_location")]
    public end_location[] end_location
    {
        get
        {
            return this.end_locationField;
        }
        set
        {
            this.end_locationField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("polyline", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public DirectionsResponseRouteLegStepPolyline[] polyline
    {
        get
        {
            return this.polylineField;
        }
        set
        {
            this.polylineField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("duration")]
    public duration[] duration
    {
        get
        {
            return this.durationField;
        }
        set
        {
            this.durationField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("distance")]
    public distance[] distance
    {
        get
        {
            return this.distanceField;
        }
        set
        {
            this.distanceField = value;
        }
    }
}
}