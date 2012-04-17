namespace DesktopClient.Entities
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
    public partial class NewDataSet {
    
        private object[] itemsField;
    
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("DirectionsResponse", typeof(DirectionsResponse))]
        [System.Xml.Serialization.XmlElementAttribute("distance", typeof(distance))]
        [System.Xml.Serialization.XmlElementAttribute("duration", typeof(duration))]
        [System.Xml.Serialization.XmlElementAttribute("end_location", typeof(end_location))]
        [System.Xml.Serialization.XmlElementAttribute("start_location", typeof(start_location))]
        public object[] Items {
            get {
                return this.itemsField;
            }
            set {
                this.itemsField = value;
            }
        }
    }
}