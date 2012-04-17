using System.Collections.Generic;
using System.Xml.Serialization;
namespace eAd.DataViewModels
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
    public partial class FilesModel {
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("file", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public List<RequiredFileModel> Items { get; set; }
    }
}