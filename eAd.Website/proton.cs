namespace eAd.Website
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
    public partial class proton
    {
        private protonCurrent current;
        private protonHistory history;
        private information information;
        private protonLocation location;
        private session session;
        private protonUser user;
        private protonVehicle vehicle;

        //  private object[] itemsField;

        //  private ItemsChoiceType[] itemsElementNameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("current", typeof(protonCurrent), Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public protonCurrent Current
        {
            get
            {
                return this.current;
            }
            set
            {
                this.current = value;
            }
        }
        [System.Xml.Serialization.XmlElementAttribute("history", typeof(protonHistory), Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public protonHistory History
        {
            get
            {
                return this.history;
            }
            set
            {
                this.history = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute("information", typeof(information))]
        public information Information
        {
            get
            {
                return this.information;
            }
            set
            {
                this.information = value;
            }
        }
        [System.Xml.Serialization.XmlElementAttribute("location", typeof(protonLocation), Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public protonLocation Location
        {
            get
            {
                return this.location;
            }
            set
            {
                this.location = value;
            }
        }
        [System.Xml.Serialization.XmlElementAttribute("session", typeof(session))]
        public session Session
        {
            get
            {
                return this.session;
            }
            set
            {
                this.session = value;
            }
        }
        [System.Xml.Serialization.XmlElementAttribute("user", typeof(protonUser), Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public protonUser User
        {
            get
            {
                return this.user;
            }
            set
            {
                this.user = value;
            }
        }
        [System.Xml.Serialization.XmlElementAttribute("vehicle", typeof(protonVehicle), Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public protonVehicle Vehicle
        {
            get
            {
                return this.vehicle;
            }
            set
            {
                this.vehicle = value;
            }
        }
        // [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
        // public object[] Items {
        //   get {
        //        return this.itemsField;
        //    }
        //  set {
        //       this.itemsField = value;
        //   }
        //  }

        ///// <remarks/>
        //[System.Xml.Serialization.XmlElementAttribute("ItemsElementName")]
        //[System.Xml.Serialization.XmlIgnoreAttribute()]
        //public ItemsChoiceType[] ItemsElementName {
        //    get {
        //        return this.itemsElementNameField;
        //    }
        //    set {
        //        this.itemsElementNameField = value;
        //    }
        //}
    }
}