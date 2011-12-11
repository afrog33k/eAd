using System;

namespace eAd.DataViewModels
{
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
