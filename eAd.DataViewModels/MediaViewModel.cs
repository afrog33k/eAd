using System;
using System.Collections.Generic;

namespace eAd.DataViewModels
{
   
    public class MediaListModel
    {
        public Int64 MediaID
        {
            get
            {
                return _MediaID;
            }
            set
            {
                _MediaID = value;

            }
        }
        private global::System.Int64 _MediaID;


        public TimeSpan Duration
        {
            get
            {
                return _duration;
            }
            set
            {
                _duration = value;

            }
        }
        private TimeSpan _duration;


        public global::System.String Location
        {
            get
            {
                return _Location;
            }
            set
            {

                _Location = value;

            }
        }

        public bool Downloaded { get; set; }

        private global::System.String _Location;
    }

    public class MediaViewModel
    {
        public static MediaViewModel Empty
        {
            get
            {
                return new MediaViewModel
                           {
                               Name = "Invalid Media"
                           };
            }
        }
      

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
      
        public Int64 MediaID
        {
            get
            {
                return _MediaID;
            }
            set
            {
                _MediaID = value;
             
            }
        }
        private global::System.Int64 _MediaID;
    
      
        public global::System.String Name
        {
            get
            {
                return _Name;
            }
            set
            {
              
                _Name =value;
          
            }
        }
        private global::System.String _Name;
     
    
        public global::System.String Location
        {
            get
            {
                return _Location;
            }
            set
            {
              
                _Location =value;
             
            }
        }
        private global::System.String _Location;
   

   
        public global::System.String Tags
        {
            get
            {
                return _Tags;
            }
            set
            {
              
                _Tags = value;
             
            }
        }
        private global::System.String _Tags;
      
        
        public global::System.String Type
        {
            get
            {
                return _Type;
            }
            set
            {
           
                _Type = value;
          
            }
        }
        private global::System.String _Type;
       

    
    }
}