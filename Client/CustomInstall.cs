using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.AccessControl;

namespace ClientApp
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

         // Get the directory info for the existing folder
        DirectoryInfo dirInfo = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

        // Now apply user access permissions to the folder
        if (dirInfo != null)
        {
            DirectorySecurity security = dirInfo.GetAccessControl();
            security.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, InheritanceFlags.ContainerInherit, PropagationFlags.None, AccessControlType.Allow));
            security.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
            dirInfo.SetAccessControl(security);
        }

        Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        Process.Start(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\eAd Client.exe");
        // Very important! Removes all those nasty temp files.
        base.Dispose();


    }
}
}
