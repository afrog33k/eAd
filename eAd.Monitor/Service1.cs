using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;

namespace eAd.Monitor
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        private static void Launch()
        {

            var app = "C:\\Program Files (x86)\\GreenCore Solutions\\eAd Desktop Client" + "\\DesktopClient.exe";

            if (File.Exists(app))
            {


                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = app;
                psi.WindowStyle = ProcessWindowStyle.Normal;

                Process p = new Process();
                p.StartInfo = psi;
                p.EnableRaisingEvents = true;
                p.Exited += LaunchAgain;

                p.Start();
            }
        }

        private static void LaunchAgain(object o, EventArgs e)
        {
            //  EventLog.WriteEntry("My simple service started.");
            Launch();
        }


        protected override void OnStart(string[] args)
        {
            Launch();
        }

        protected override void OnStop()
        {
        }
    }
}
