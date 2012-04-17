using System.Collections.Generic;

namespace eAd.DataViewModels
{
    /// <remarks/>
  
    [System.SerializableAttribute()]

    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
    public partial class ScheduleModel {
        /// <remarks/>
       public List<ScheduleLayout> Items { get; set; }
    }
    
    /// <remarks/>
     
        [System.SerializableAttribute()]
      
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public class ScheduleLayout
        {
            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string File { get; set; }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string FromDate { get; set; }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string ToDate { get; set; }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public int ScheduleId { get; set; }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public int Priority { get; set; }

            public bool Default { get; set; }
        }
    }
