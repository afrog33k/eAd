using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;


namespace eAd.ClientConfiguration
{
    [RunInstaller(true)]
    public partial class CustomInstall : System.Configuration.Install.Installer
    {
        public CustomInstall()
        {
            InitializeComponent();
        }

        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);

            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            Process.Start(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\eAd.ClientConfiguration.exe");
            // Very important! Removes all those nasty temp files.
            base.Dispose();

          
        }
    }
}
