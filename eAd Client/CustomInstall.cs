using System.Threading;

namespace ClientApp
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Configuration.Install;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Security.AccessControl;

    [RunInstaller(true)]
    public class CustomInstall : Installer
    {
        private IContainer components;

        public CustomInstall()
        {
            this.InitializeComponent();
        }

        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
            DirectoryInfo info = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            if (info != null)
            {
                DirectorySecurity accessControl = info.GetAccessControl();
                accessControl.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, InheritanceFlags.ContainerInherit, PropagationFlags.None, AccessControlType.Allow));
                accessControl.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                info.SetAccessControl(accessControl);
            }
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            Process.Start(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\eAd Client.exe");
            ThreadPool.QueueUserWorkItem(
                (state)=>
                    {
                        Thread.Sleep(3000);
                        Environment.Exit(0);
                    }
                );
            base.Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new Container();
        }

        protected override void OnAfterInstall(IDictionary savedState)
        {
            base.OnAfterInstall(savedState);
        }
    }
}

