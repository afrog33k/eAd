using System.Security.Permissions;
using System.Runtime.InteropServices;
using DesktopClient.Menu;

namespace DesktopClient
{
[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
[ComVisible(true)]
public class ObjectForScriptingHelper
{
    CustomerPage mExternalWPF;
    public ObjectForScriptingHelper(CustomerPage w)
    {
        this.mExternalWPF = w;
    }

    public void InvokeMeFromJavascript(string jsscript)
    {
        //this.mExternalWPF.tbMessageFromBrowser.Text = string.Format("Message :{0}", jsscript);

    }
}
}
