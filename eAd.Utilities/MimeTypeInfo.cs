using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace eAd.Utilities
{
    [Serializable]
    public class MimeTypeInfo
    {
        // Methods
        public MimeTypeInfo()
        {
        }

        public MimeTypeInfo(string mimeType, List<string> extensions)
        {
            this.MimeType = mimeType;
            this.Extensions = extensions;
        }

        // Properties
        [XmlElement("extension")]
        public List<string> Extensions
        {
            get;
            set;
        }

        [XmlAttribute(AttributeName = "mimeType")]
        public string MimeType
        {
            get;
            set;
        }
    }
}