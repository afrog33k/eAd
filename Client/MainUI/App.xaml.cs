using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using Client.Core;

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
        get
        {
            return System.AppDomain.CurrentDomain.BaseDirectory;
        }
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

    public static bool ShutDownRequested { get; set; }


    public static void DoEvents()
    {

    }

    public static void Close()
    {
        ShutDownRequested = true;
        Application.Current.Shutdown();
    }

    public App()
    {

        base.StartupUri = new Uri("PageSwitcher.xaml", UriKind.Relative);

        base.ShutdownMode = ShutdownMode.OnMainWindowClose;

        bool createdNew = true;

        using (new Mutex(true, "eAd Advert Platform", out createdNew))
        {

            if (createdNew)
            {

                base.Run();

            }

            else
            {

                Process currentProcess = Process.GetCurrentProcess();

                foreach (Process process2 in Process.GetProcessesByName(currentProcess.ProcessName))
                {

                    if (process2.Id != currentProcess.Id)
                    {

                        SetForegroundWindow(process2.MainWindowHandle);

                        return;

                    }

                }

            }

        }

    }

    private static void RunApp(string[] arguments)
    {

        bool createdNew = true;

        new Mutex(true, "UniqueApplicationName", out createdNew);

        if (createdNew)
        {

            Splasher.Splash = new SplashScreen();

            Splasher.ShowSplash();

            new App();

        }

    }


    [return: MarshalAs(UnmanagedType.Bool)]

    [DllImport("user32.dll")]

    private static extern bool SetForegroundWindow(IntPtr hWnd);

    public static void SingleInstanceArgumentsReceived(object sender, ArgumentsReceivedEventArgs e)
    {

        //if (PageSwitcher.Instance != null)
        //{

        //    PageSwitcher.Instance.RunCommands(e.Args);

        //}

    }



}
}
