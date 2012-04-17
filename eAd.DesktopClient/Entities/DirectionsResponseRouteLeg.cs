namespace DesktopClient.Entities
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class DirectionsResponseRouteLeg {
    
        private string start_addressField;
    
        private string end_addressField;
    
        private DirectionsResponseRouteLegStep[] stepField;
    
        private duration[] durationField;
    
        private distance[] distanceField;
    
        private start_location[] start_locationField;
    
        private end_location[] end_locationField;
    
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string start_address {
            get {
                return this.start_addressField;
            }
            set {
                this.start_addressField = value;
            }
        }
    
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string end_address {
            get {
                return this.end_addressField;
            }
            set {
                this.end_addressField = value;
            }
        }
    
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("step", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DirectionsResponseRouteLegStep[] step {
            get {
                return this.stepField;
            }
            set {
                this.stepField = value;
            }
        }
    
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("duration")]
        public duration[] duration {
            get {
                return this.durationField;
            }
            set {
                this.durationField = value;
            }
        }
    
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("distance")]
        public distance[] distance {
            get {
                return this.distanceField;
            }
            set {
                this.distanceField = value;
            }
        }
    
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("start_location")]
        public start_location[] start_location {
            get {
                return this.start_locationField;
            }
            set {
                this.start_locationField = value;
            }
        }
    
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("end_location")]
        public end_location[] end_location {
            get {
                return this.end_locationField;
            }
            set {
                this.end_locationField = value;
            }
        }
    }
}