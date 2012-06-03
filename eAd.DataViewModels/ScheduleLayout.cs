namespace eAd.DataViewModels
{
    /// <remarks/>

    [System.SerializableAttribute()]

    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class ScheduleLayout
    {
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string File
        {
            get;
            set;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string FromDate
        {
            get;
            set;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ToDate
        {
            get;
            set;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int ScheduleId
        {
            get;
            set;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int Priority
        {
            get;
            set;
        }

        public bool Default
        {
            get;
            set;
        }

        public string Hash
        {
            get;
            set;
        }

        public string Type { get; set; }
    }
}