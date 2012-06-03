namespace eAd.Website
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class protonLocation
    {

        private information[] informationField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("information")]
        public information[] information
        {
            get
            {
                return this.informationField;
            }
            set
            {
                this.informationField = value;
            }
        }
    }
}