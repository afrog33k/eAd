using System;
using System.Collections.Generic;

namespace eAd.DataViewModels
{

  
        public class PositionViewModel
        {
            #region Factory Method

            /// <summary>
            /// Create a new Position object.
            /// </summary>
            /// <param name="positionID">Initial value of the PositionID property.</param>
            public static PositionViewModel CreatePosition(Int64 positionID)
            {
                PositionViewModel position = new PositionViewModel();
                position.PositionID = positionID;
                return position;
            }

            #endregion
            #region Primitive Properties

            /// <summary>
            /// No Metadata Documentation available.
            /// </summary>
         
            public Int64 PositionID
            {
                get
                {
                    return _positionID;
                }
                set
                {
                    if (_positionID != value)
                    {
                       
                        _positionID = (value);
                      
                    }
                }
            }
            private global::System.Int64 _positionID;
          

            /// <summary>
            /// No Metadata Documentation available.
            /// </summary>
          
            public String Name
            {
                get
                {
                    return _name;
                }
                set
                {
                 
                    _name = (value);
                
                }
            }
            private global::System.String _name;
           
            /// <summary>
            /// No Metadata Documentation available.
            /// </summary>
         
            public global::System.String ContentURL
            {
                get
                {
                    return _ContentURL;
                }
                set
                {
             
                    _ContentURL = (value);
                }
            }
            private global::System.String _ContentURL;
        
            public Nullable<global::System.Int64> MosaicID
            {
                get
                {
                    return _mosaicID;
                }
                set { _mosaicID = (value); }
            }
            private Nullable<Int64> _mosaicID;
         

            /// <summary>
            /// No Metadata Documentation available.
            /// </summary>
         
            public Nullable<global::System.Double> X
            {
                get
                {
                    return _x;
                }
                set
                {
                  
                    _x = (value);
                  
                }
            }
            private Nullable<Double> _x;
          

            /// <summary>
            /// No Metadata Documentation available.
            /// </summary>
        
            public Nullable<Double> Y
            {
                get
                {
                    return _y;
                }
                set
                {
                  
                    _y = (value);
                }
            }
            private Nullable<Double> _y;
         

            /// <summary>
            /// No Metadata Documentation available.
            /// </summary>
        
            public Nullable<Double> Width
            {
                get
                {
                    return _width;
                }
                set
                {
                 
                    _width = (value);
                }
            }
            private Nullable<Double> _width;
         

            /// <summary>
            /// No Metadata Documentation available.
            /// </summary>
          
            public Nullable<Double> Height
            {
                get
                {
                    return _height;
                }
                set
                {
               
                    _height = (value);
                }
            }

            public List<long> Media { get; set; }

            private Nullable<Double> _height;
         

            #endregion

          

        }
    
    public class CustomerViewModel
    {
        public static CustomerViewModel Empty
        {
            get
            {
                return new CustomerViewModel
                           {
                               Name = "Invalid Customer"
                           };
            }
        }
        private string _email;
        private DateTime? _lastRechargeDate;
        private string _accountBalance;

        public long CustomerID
        {
            get;
            set;
        }
        public string RFID
        {
            get;
            set;
        }


       

        public string CarLicense
        {
            get;
            set;
        }

        public string LastBillAmount
        {
            get;
            set;
        }
        public string ChargeRemaining
        {
            get;
            set;
        }
        public string CarMake
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public string CarModel
        {
            get;
            set;
        }

        public string Email
        {
            get {
                return _email;
            }
            set {
                _email = value;
            }
        }

        public string Picture { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public DateTime? LastRechargeDate
        {
            get {
                return _lastRechargeDate;
            }
            set {
                _lastRechargeDate = value;
            }
        }

        public string AccountBalance
        {
            get {
                return _accountBalance;
            }
            set {
                _accountBalance = value;
            }
        }
    }
}
