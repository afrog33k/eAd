using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Client
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
        Process.Start(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Client.exe");
        // Very important! Removes all those nasty temp files.
        base.Dispose();


    }
}
}
