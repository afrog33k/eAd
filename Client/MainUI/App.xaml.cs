using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static string _productName;

        public static string UserAppDataPath
        {
            get { return System.AppDomain.CurrentDomain.BaseDirectory; }
        }

        public static string ProductName
        {
            get
            {
                return "eAd Desktop";
            }
            set
            {
                _productName = value;
            }
        }



        public static void DoEvents()
        {

        }
    }
}
