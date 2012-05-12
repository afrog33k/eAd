// Type: System.Windows.Forms.Application
// Assembly: System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.Windows.Forms.dll

using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms
{
public sealed class Application
{
    #region Delegates

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public delegate bool MessageLoopCallback();

    #endregion

    public static bool AllowQuit
    {
        get;
    }
    public static RegistryKey CommonAppDataRegistry
    {
        get;
    }
    public static string CommonAppDataPath
    {
        get;
    }
    public static string CompanyName
    {
        get;
    }
    public static CultureInfo CurrentCulture
    {
        get;
        set;
    }
    public static InputLanguage CurrentInputLanguage
    {
        get;
        set;
    }
    public static string ExecutablePath
    {
        get;
    }
    public static string LocalUserAppDataPath
    {
        get;
    }
    public static bool MessageLoop
    {
        get;
    }

    public static FormCollection OpenForms
    {
        [UIPermission(SecurityAction.Demand, Window = UIPermissionWindow.AllWindows)]
        get;
    }

    public static string ProductName
    {
        get;
    }
    public static string ProductVersion
    {
        get;
    }
    public static bool RenderWithVisualStyles
    {
        get;
    }
    public static string SafeTopLevelCaptionFormat
    {
        get;
        set;
    }
    public static string StartupPath
    {
        get;
    }
    public static bool UseWaitCursor
    {
        get;
        set;
    }
    public static string UserAppDataPath
    {
        get;
    }
    public static RegistryKey UserAppDataRegistry
    {
        get;
    }
    public static VisualStyleState VisualStyleState
    {
        get;
        set;
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public static void RegisterMessageLoop(Application.MessageLoopCallback callback);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public static void UnregisterMessageLoop();

    public static void AddMessageFilter(IMessageFilter value);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public static bool FilterMessage(ref Message message);

    public static void DoEvents();
    public static void EnableVisualStyles();
    public static void Exit();

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static void Exit(CancelEventArgs e);

    public static void ExitThread();
    public static ApartmentState OleRequired();
    public static void OnThreadException(Exception t);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public static void RaiseIdle(EventArgs e);

    public static void RemoveMessageFilter(IMessageFilter value);

    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public static void Restart();

    public static void Run();
    public static void Run(Form mainForm);
    public static void Run(ApplicationContext context);
    public static void SetCompatibleTextRenderingDefault(bool defaultValue);
    public static bool SetSuspendState(PowerState state, bool force, bool disableWakeEvent);
    public static void SetUnhandledExceptionMode(UnhandledExceptionMode mode);
    public static void SetUnhandledExceptionMode(UnhandledExceptionMode mode, bool threadScope);
    public static event EventHandler ApplicationExit;
    public static event EventHandler Idle;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static event EventHandler EnterThreadModal;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static event EventHandler LeaveThreadModal;

    public static event ThreadExceptionEventHandler ThreadException;
    public static event EventHandler ThreadExit;
}
}
