using System.Collections.Generic;

namespace eAd.DataViewModels
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
  
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public class LayoutRegion {
        /// <remarks/>
      //  [System.Xml.Serialization.XmlElementAttribute("media", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public List<LayoutRegionMedia> Media { get; set; }

        /// <remarks/>
     //   [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Id { get; set; }

        /// <remarks/>
    //    [System.Xml.Serialization.XmlAttributeAttribute()]
        public int UserId { get; set; }

        /// <remarks/>
      //  [System.Xml.Serialization.XmlAttributeAttribute()]
        public int Width { get; set; }

        /// <remarks/>
  //      [System.Xml.Serialization.XmlAttributeAttribute()]
        public int Height { get; set; }

        /// <remarks/>
     //   [System.Xml.Serialization.XmlAttributeAttribute()]
        public int Top { get; set; }

        /// <remarks/>
    //    [System.Xml.Serialization.XmlAttributeAttribute()]
        public int Left { get; set; }

        public override string ToString()
        {
            return Id;
        }
    }
}