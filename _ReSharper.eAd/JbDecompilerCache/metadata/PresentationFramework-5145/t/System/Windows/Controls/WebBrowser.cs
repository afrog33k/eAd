// Type: System.Windows.Controls.WebBrowser
// Assembly: PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\WPF\PresentationFramework.dll

using System;
using System.IO;
using System.Runtime;
using System.Security;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Navigation;

namespace System.Windows.Controls
{
public sealed class WebBrowser : ActiveXHost
{
    [SecurityCritical]
    public WebBrowser();

    public Uri Source
    {
        [SecurityCritical]
        get;
        set;
    }

    public bool CanGoBack
    {
        get;
    }
    public bool CanGoForward
    {
        get;
    }

    public object ObjectForScripting
    {
        get;
        [SecurityCritical]
        set;
    }

    public object Document
    {
        [SecurityCritical]
        get;
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public void Navigate(Uri source);

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public void Navigate(string source);

    public void Navigate(Uri source, string targetFrameName, byte[] postData, string additionalHeaders);
    public void Navigate(string source, string targetFrameName, byte[] postData, string additionalHeaders);
    public void NavigateToStream(Stream stream);
    public void NavigateToString(string text);

    [SecurityCritical]
    public void GoBack();

    [SecurityCritical]
    public void GoForward();

    [SecurityCritical]
    public void Refresh();

    [SecurityCritical]
    public void Refresh(bool noCache);

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public object InvokeScript(string scriptName);

    [SecurityCritical]
    public object InvokeScript(string scriptName, params object[] args);

    [SecurityTreatAsSafe]
    [SecurityCritical]
    protected override bool TranslateAcceleratorCore(ref MSG msg, ModifierKeys modifiers);

    [SecurityCritical]
    protected override bool TabIntoCore(TraversalRequest request);

    public event NavigatingCancelEventHandler Navigating;
    public event NavigatedEventHandler Navigated;
    public event LoadCompletedEventHandler LoadCompleted;
}
}
