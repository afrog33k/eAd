namespace DesktopClient.Entities
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class DirectionsResponseRoute {
    
        private string summaryField;
    
        private string copyrightsField;
    
        private DirectionsResponseRouteLeg[] legField;
    
        private DirectionsResponseRouteOverview_polyline[] overview_polylineField;
    
        private DirectionsResponseRouteWaypoint_index[] waypoint_indexField;
    
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string summary {
            get {
                return this.summaryField;
            }
            set {
                this.summaryField = value;
            }
        }
    
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string copyrights {
            get {
                return this.copyrightsField;
            }
            set {
                this.copyrightsField = value;
            }
        }
    
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("leg", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DirectionsResponseRouteLeg[] leg {
            get {
                return this.legField;
            }
            set {
                this.legField = value;
            }
        }
    
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("overview_polyline", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DirectionsResponseRouteOverview_polyline[] overview_polyline {
            get {
                return this.overview_polylineField;
            }
            set {
                this.overview_polylineField = value;
            }
        }
    
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("waypoint_index", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public DirectionsResponseRouteWaypoint_index[] waypoint_index {
            get {
                return this.waypoint_indexField;
            }
            set {
                this.waypoint_indexField = value;
            }
        }
    }
}