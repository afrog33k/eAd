﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Data.EntityClient;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Runtime.Serialization;

[assembly: EdmSchemaAttribute()]
#region EDM Relationship Metadata

[assembly: EdmRelationshipAttribute("eAdDataModel", "FK_CarCustomer", "Customers", System.Data.Metadata.Edm.RelationshipMultiplicity.One, typeof(eAd.DataAccess.Customer), "Cars", System.Data.Metadata.Edm.RelationshipMultiplicity.Many, typeof(eAd.DataAccess.Car), true)]

#endregion

namespace eAd.DataAccess
{
    #region Contexts
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    public partial class eAdDataEntities : ObjectContext
    {
        #region Constructors
    
        /// <summary>
        /// Initializes a new eAdDataEntities object using the connection string found in the 'eAdDataEntities' section of the application configuration file.
        /// </summary>
        public eAdDataEntities() : base("name=eAdDataEntities", "eAdDataEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new eAdDataEntities object.
        /// </summary>
        public eAdDataEntities(string connectionString) : base(connectionString, "eAdDataEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new eAdDataEntities object.
        /// </summary>
        public eAdDataEntities(EntityConnection connection) : base(connection, "eAdDataEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        #endregion
    
        #region Partial Methods
    
        partial void OnContextCreated();
    
        #endregion
    
        #region ObjectSet Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectSet<Car> Cars
        {
            get
            {
                if ((_Cars == null))
                {
                    _Cars = base.CreateObjectSet<Car>("Cars");
                }
                return _Cars;
            }
        }
        private ObjectSet<Car> _Cars;
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectSet<Customer> Customers
        {
            get
            {
                if ((_Customers == null))
                {
                    _Customers = base.CreateObjectSet<Customer>("Customers");
                }
                return _Customers;
            }
        }
        private ObjectSet<Customer> _Customers;
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectSet<Message> Messages
        {
            get
            {
                if ((_Messages == null))
                {
                    _Messages = base.CreateObjectSet<Message>("Messages");
                }
                return _Messages;
            }
        }
        private ObjectSet<Message> _Messages;
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectSet<Station> Stations
        {
            get
            {
                if ((_Stations == null))
                {
                    _Stations = base.CreateObjectSet<Station>("Stations");
                }
                return _Stations;
            }
        }
        private ObjectSet<Station> _Stations;

        #endregion

        #region AddTo Methods
    
        /// <summary>
        /// Deprecated Method for adding a new object to the Cars EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddToCars(Car car)
        {
            base.AddObject("Cars", car);
        }
    
        /// <summary>
        /// Deprecated Method for adding a new object to the Customers EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddToCustomers(Customer customer)
        {
            base.AddObject("Customers", customer);
        }
    
        /// <summary>
        /// Deprecated Method for adding a new object to the Messages EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddToMessages(Message message)
        {
            base.AddObject("Messages", message);
        }
    
        /// <summary>
        /// Deprecated Method for adding a new object to the Stations EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddToStations(Station station)
        {
            base.AddObject("Stations", station);
        }

        #endregion

    }

    #endregion

    #region Entities
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="eAdDataModel", Name="Car")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class Car : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new Car object.
        /// </summary>
        /// <param name="customerID">Initial value of the CustomerID property.</param>
        /// <param name="carID">Initial value of the CarID property.</param>
        public static Car CreateCar(global::System.Int64 customerID, global::System.Int64 carID)
        {
            Car car = new Car();
            car.CustomerID = customerID;
            car.CarID = carID;
            return car;
        }

        #endregion

        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int64 CustomerID
        {
            get
            {
                return _CustomerID;
            }
            set
            {
                OnCustomerIDChanging(value);
                ReportPropertyChanging("CustomerID");
                _CustomerID = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("CustomerID");
                OnCustomerIDChanged();
            }
        }
        private global::System.Int64 _CustomerID;
        partial void OnCustomerIDChanging(global::System.Int64 value);
        partial void OnCustomerIDChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String Make
        {
            get
            {
                return _Make;
            }
            set
            {
                OnMakeChanging(value);
                ReportPropertyChanging("Make");
                _Make = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("Make");
                OnMakeChanged();
            }
        }
        private global::System.String _Make;
        partial void OnMakeChanging(global::System.String value);
        partial void OnMakeChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String Model
        {
            get
            {
                return _Model;
            }
            set
            {
                OnModelChanging(value);
                ReportPropertyChanging("Model");
                _Model = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("Model");
                OnModelChanged();
            }
        }
        private global::System.String _Model;
        partial void OnModelChanging(global::System.String value);
        partial void OnModelChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String License
        {
            get
            {
                return _License;
            }
            set
            {
                OnLicenseChanging(value);
                ReportPropertyChanging("License");
                _License = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("License");
                OnLicenseChanged();
            }
        }
        private global::System.String _License;
        partial void OnLicenseChanging(global::System.String value);
        partial void OnLicenseChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String BatteryCharge
        {
            get
            {
                return _BatteryCharge;
            }
            set
            {
                OnBatteryChargeChanging(value);
                ReportPropertyChanging("BatteryCharge");
                _BatteryCharge = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("BatteryCharge");
                OnBatteryChargeChanged();
            }
        }
        private global::System.String _BatteryCharge;
        partial void OnBatteryChargeChanging(global::System.String value);
        partial void OnBatteryChargeChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.DateTime> LastRechargeDate
        {
            get
            {
                return _LastRechargeDate;
            }
            set
            {
                OnLastRechargeDateChanging(value);
                ReportPropertyChanging("LastRechargeDate");
                _LastRechargeDate = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("LastRechargeDate");
                OnLastRechargeDateChanged();
            }
        }
        private Nullable<global::System.DateTime> _LastRechargeDate;
        partial void OnLastRechargeDateChanging(Nullable<global::System.DateTime> value);
        partial void OnLastRechargeDateChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int64 CarID
        {
            get
            {
                return _CarID;
            }
            set
            {
                if (_CarID != value)
                {
                    OnCarIDChanging(value);
                    ReportPropertyChanging("CarID");
                    _CarID = StructuralObject.SetValidValue(value);
                    ReportPropertyChanged("CarID");
                    OnCarIDChanged();
                }
            }
        }
        private global::System.Int64 _CarID;
        partial void OnCarIDChanging(global::System.Int64 value);
        partial void OnCarIDChanged();

        #endregion

    
        #region Navigation Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [XmlIgnoreAttribute()]
        [SoapIgnoreAttribute()]
        [DataMemberAttribute()]
        [EdmRelationshipNavigationPropertyAttribute("eAdDataModel", "FK_CarCustomer", "Customers")]
        public Customer Customer
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedReference<Customer>("eAdDataModel.FK_CarCustomer", "Customers").Value;
            }
            set
            {
                ((IEntityWithRelationships)this).RelationshipManager.GetRelatedReference<Customer>("eAdDataModel.FK_CarCustomer", "Customers").Value = value;
            }
        }
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [BrowsableAttribute(false)]
        [DataMemberAttribute()]
        public EntityReference<Customer> CustomerReference
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedReference<Customer>("eAdDataModel.FK_CarCustomer", "Customers");
            }
            set
            {
                if ((value != null))
                {
                    ((IEntityWithRelationships)this).RelationshipManager.InitializeRelatedReference<Customer>("eAdDataModel.FK_CarCustomer", "Customers", value);
                }
            }
        }

        #endregion

    }
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="eAdDataModel", Name="Customer")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class Customer : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new Customer object.
        /// </summary>
        /// <param name="customerID">Initial value of the CustomerID property.</param>
        public static Customer CreateCustomer(global::System.Int64 customerID)
        {
            Customer customer = new Customer();
            customer.CustomerID = customerID;
            return customer;
        }

        #endregion

        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String Name
        {
            get
            {
                return _Name;
            }
            set
            {
                OnNameChanging(value);
                ReportPropertyChanging("Name");
                _Name = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("Name");
                OnNameChanged();
            }
        }
        private global::System.String _Name;
        partial void OnNameChanging(global::System.String value);
        partial void OnNameChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String RFID
        {
            get
            {
                return _RFID;
            }
            set
            {
                OnRFIDChanging(value);
                ReportPropertyChanging("RFID");
                _RFID = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("RFID");
                OnRFIDChanged();
            }
        }
        private global::System.String _RFID;
        partial void OnRFIDChanging(global::System.String value);
        partial void OnRFIDChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String Balance
        {
            get
            {
                return _Balance;
            }
            set
            {
                OnBalanceChanging(value);
                ReportPropertyChanging("Balance");
                _Balance = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("Balance");
                OnBalanceChanged();
            }
        }
        private global::System.String _Balance;
        partial void OnBalanceChanging(global::System.String value);
        partial void OnBalanceChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String LastBillAmount
        {
            get
            {
                return _LastBillAmount;
            }
            set
            {
                OnLastBillAmountChanging(value);
                ReportPropertyChanging("LastBillAmount");
                _LastBillAmount = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("LastBillAmount");
                OnLastBillAmountChanged();
            }
        }
        private global::System.String _LastBillAmount;
        partial void OnLastBillAmountChanging(global::System.String value);
        partial void OnLastBillAmountChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String Picture
        {
            get
            {
                return _Picture;
            }
            set
            {
                OnPictureChanging(value);
                ReportPropertyChanging("Picture");
                _Picture = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("Picture");
                OnPictureChanged();
            }
        }
        private global::System.String _Picture;
        partial void OnPictureChanging(global::System.String value);
        partial void OnPictureChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String Email
        {
            get
            {
                return _Email;
            }
            set
            {
                OnEmailChanging(value);
                ReportPropertyChanging("Email");
                _Email = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("Email");
                OnEmailChanged();
            }
        }
        private global::System.String _Email;
        partial void OnEmailChanging(global::System.String value);
        partial void OnEmailChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String Phone
        {
            get
            {
                return _Phone;
            }
            set
            {
                OnPhoneChanging(value);
                ReportPropertyChanging("Phone");
                _Phone = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("Phone");
                OnPhoneChanged();
            }
        }
        private global::System.String _Phone;
        partial void OnPhoneChanging(global::System.String value);
        partial void OnPhoneChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String Address
        {
            get
            {
                return _Address;
            }
            set
            {
                OnAddressChanging(value);
                ReportPropertyChanging("Address");
                _Address = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("Address");
                OnAddressChanged();
            }
        }
        private global::System.String _Address;
        partial void OnAddressChanging(global::System.String value);
        partial void OnAddressChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int64 CustomerID
        {
            get
            {
                return _CustomerID;
            }
            set
            {
                if (_CustomerID != value)
                {
                    OnCustomerIDChanging(value);
                    ReportPropertyChanging("CustomerID");
                    _CustomerID = StructuralObject.SetValidValue(value);
                    ReportPropertyChanged("CustomerID");
                    OnCustomerIDChanged();
                }
            }
        }
        private global::System.Int64 _CustomerID;
        partial void OnCustomerIDChanging(global::System.Int64 value);
        partial void OnCustomerIDChanged();

        #endregion

    
        #region Navigation Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [XmlIgnoreAttribute()]
        [SoapIgnoreAttribute()]
        [DataMemberAttribute()]
        [EdmRelationshipNavigationPropertyAttribute("eAdDataModel", "FK_CarCustomer", "Cars")]
        public EntityCollection<Car> Cars
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedCollection<Car>("eAdDataModel.FK_CarCustomer", "Cars");
            }
            set
            {
                if ((value != null))
                {
                    ((IEntityWithRelationships)this).RelationshipManager.InitializeRelatedCollection<Car>("eAdDataModel.FK_CarCustomer", "Cars", value);
                }
            }
        }

        #endregion

    }
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="eAdDataModel", Name="Message")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class Message : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new Message object.
        /// </summary>
        /// <param name="messageID">Initial value of the MessageID property.</param>
        /// <param name="type">Initial value of the Type property.</param>
        /// <param name="sent">Initial value of the Sent property.</param>
        /// <param name="stationID">Initial value of the StationID property.</param>
        /// <param name="userID">Initial value of the UserID property.</param>
        /// <param name="command">Initial value of the Command property.</param>
        public static Message CreateMessage(global::System.Int64 messageID, global::System.String type, global::System.Boolean sent, global::System.Int64 stationID, global::System.Int64 userID, global::System.String command)
        {
            Message message = new Message();
            message.MessageID = messageID;
            message.Type = type;
            message.Sent = sent;
            message.StationID = stationID;
            message.UserID = userID;
            message.Command = command;
            return message;
        }

        #endregion

        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int64 MessageID
        {
            get
            {
                return _MessageID;
            }
            set
            {
                if (_MessageID != value)
                {
                    OnMessageIDChanging(value);
                    ReportPropertyChanging("MessageID");
                    _MessageID = StructuralObject.SetValidValue(value);
                    ReportPropertyChanged("MessageID");
                    OnMessageIDChanged();
                }
            }
        }
        private global::System.Int64 _MessageID;
        partial void OnMessageIDChanging(global::System.Int64 value);
        partial void OnMessageIDChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String Type
        {
            get
            {
                return _Type;
            }
            set
            {
                OnTypeChanging(value);
                ReportPropertyChanging("Type");
                _Type = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("Type");
                OnTypeChanged();
            }
        }
        private global::System.String _Type;
        partial void OnTypeChanging(global::System.String value);
        partial void OnTypeChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String Text
        {
            get
            {
                return _Text;
            }
            set
            {
                OnTextChanging(value);
                ReportPropertyChanging("Text");
                _Text = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("Text");
                OnTextChanged();
            }
        }
        private global::System.String _Text;
        partial void OnTextChanging(global::System.String value);
        partial void OnTextChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Boolean Sent
        {
            get
            {
                return _Sent;
            }
            set
            {
                OnSentChanging(value);
                ReportPropertyChanging("Sent");
                _Sent = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("Sent");
                OnSentChanged();
            }
        }
        private global::System.Boolean _Sent;
        partial void OnSentChanging(global::System.Boolean value);
        partial void OnSentChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int64 StationID
        {
            get
            {
                return _StationID;
            }
            set
            {
                OnStationIDChanging(value);
                ReportPropertyChanging("StationID");
                _StationID = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("StationID");
                OnStationIDChanged();
            }
        }
        private global::System.Int64 _StationID;
        partial void OnStationIDChanging(global::System.Int64 value);
        partial void OnStationIDChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int64 UserID
        {
            get
            {
                return _UserID;
            }
            set
            {
                OnUserIDChanging(value);
                ReportPropertyChanging("UserID");
                _UserID = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("UserID");
                OnUserIDChanged();
            }
        }
        private global::System.Int64 _UserID;
        partial void OnUserIDChanging(global::System.Int64 value);
        partial void OnUserIDChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String Command
        {
            get
            {
                return _Command;
            }
            set
            {
                OnCommandChanging(value);
                ReportPropertyChanging("Command");
                _Command = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("Command");
                OnCommandChanged();
            }
        }
        private global::System.String _Command;
        partial void OnCommandChanging(global::System.String value);
        partial void OnCommandChanged();

        #endregion

    
    }
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="eAdDataModel", Name="Station")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class Station : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new Station object.
        /// </summary>
        /// <param name="stationID">Initial value of the StationID property.</param>
        public static Station CreateStation(global::System.Int64 stationID)
        {
            Station station = new Station();
            station.StationID = stationID;
            return station;
        }

        #endregion

        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String Location
        {
            get
            {
                return _Location;
            }
            set
            {
                OnLocationChanging(value);
                ReportPropertyChanging("Location");
                _Location = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("Location");
                OnLocationChanged();
            }
        }
        private global::System.String _Location;
        partial void OnLocationChanging(global::System.String value);
        partial void OnLocationChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String Name
        {
            get
            {
                return _Name;
            }
            set
            {
                OnNameChanging(value);
                ReportPropertyChanging("Name");
                _Name = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("Name");
                OnNameChanged();
            }
        }
        private global::System.String _Name;
        partial void OnNameChanging(global::System.String value);
        partial void OnNameChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Boolean> Available
        {
            get
            {
                return _Available;
            }
            set
            {
                OnAvailableChanging(value);
                ReportPropertyChanging("Available");
                _Available = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("Available");
                OnAvailableChanged();
            }
        }
        private Nullable<global::System.Boolean> _Available;
        partial void OnAvailableChanging(Nullable<global::System.Boolean> value);
        partial void OnAvailableChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int64 StationID
        {
            get
            {
                return _StationID;
            }
            set
            {
                if (_StationID != value)
                {
                    OnStationIDChanging(value);
                    ReportPropertyChanging("StationID");
                    _StationID = StructuralObject.SetValidValue(value);
                    ReportPropertyChanged("StationID");
                    OnStationIDChanged();
                }
            }
        }
        private global::System.Int64 _StationID;
        partial void OnStationIDChanging(global::System.Int64 value);
        partial void OnStationIDChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.DateTime> LastCheckIn
        {
            get
            {
                return _LastCheckIn;
            }
            set
            {
                OnLastCheckInChanging(value);
                ReportPropertyChanging("LastCheckIn");
                _LastCheckIn = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("LastCheckIn");
                OnLastCheckInChanged();
            }
        }
        private Nullable<global::System.DateTime> _LastCheckIn;
        partial void OnLastCheckInChanging(Nullable<global::System.DateTime> value);
        partial void OnLastCheckInChanged();

        #endregion

    
    }

    #endregion

    
}
