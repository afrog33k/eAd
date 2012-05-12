namespace eAd.DataViewModels
{
using System;
using System.Runtime.CompilerServices;

public class CustomerViewModel
{
    private string _accountBalance;
    private string _email;
    private DateTime? _lastRechargeDate;

    public string AccountBalance
    {
        get
        {
            return this._accountBalance;
        }
        set
        {
            this._accountBalance = value;
        }
    }

    public string Address
    {
        get;
        set;
    }

    public string CarLicense
    {
        get;
        set;
    }

    public string CarMake
    {
        get;
        set;
    }

    public string CarModel
    {
        get;
        set;
    }

    public string ChargeRemaining
    {
        get;
        set;
    }

    public long CustomerID
    {
        get;
        set;
    }

    public string Email
    {
        get
        {
            return this._email;
        }
        set
        {
            this._email = value;
        }
    }

    public static CustomerViewModel Empty
    {
        get
        {
            return new CustomerViewModel { Name = "Invalid Customer" };
        }
    }

    public string LastBillAmount
    {
        get;
        set;
    }

    public DateTime? LastRechargeDate
    {
        get
        {
            return this._lastRechargeDate;
        }
        set
        {
            this._lastRechargeDate = value;
        }
    }

    public string Name
    {
        get;
        set;
    }

    public string Phone
    {
        get;
        set;
    }

    public string Picture
    {
        get;
        set;
    }

    public string RFID
    {
        get;
        set;
    }
}
}

