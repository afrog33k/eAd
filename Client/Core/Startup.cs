using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace ClientApp.Core
{
internal class Startup
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {

        Guid identifier = new Guid("{6EAE2E61-E7EE-42bf-8EBE-BAB890C5410F}");

        using (SingleInstance instance = new SingleInstance(identifier))
        {

            if (instance.IsFirstInstance)
            {

                instance.ArgumentsReceived += new EventHandler<ArgumentsReceivedEventArgs>(App.SingleInstanceArgumentsReceived);

                instance.ListenForArgumentsFromSuccessiveInstances();

                RunApp(args);

            }

            else
            {

                instance.PassArgumentsToFirstInstance(args);

            }

        }

    }

    private static void RunApp(string[] arg)
    {


        Process[] runningProcesses = Process.GetProcessesByName("eAd Client");

        if (runningProcesses.Length <= 1)
        {
            System.Diagnostics.Trace.Listeners.Add(new ClientTraceListener());
            System.Diagnostics.Trace.AutoFlush = false;


            try
            {
                if (arg.GetLength(0) > 0)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    System.Diagnostics.Trace.WriteLine(new LogMessage("Main", "Options Started"), LogType.Info.ToString());
                    Form formMain = new OptionForm();
                    Application.Run(formMain);
                }
                else
                {
                    bool createdNew = true;

                    new Mutex(true, "UniqueApplicationName", out createdNew);

                    if (createdNew)
                    {

                        Splasher.Splash = new SplashScreen();

                        Splasher.ShowSplash();

                        Trace.WriteLine(new LogMessage("Main", "Client Started"), LogType.Info.ToString());
                        App app = new App();

                        app.Run();

                    }


                }
            }
            catch (Exception ex)
            {
                HandleUnhandledException(ex);
            }

            // Catch unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException +=
                new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);

            // Always flush at the end
            System.Diagnostics.Trace.WriteLine(new LogMessage("Main", "Application Finished"), LogType.Info.ToString());
            System.Diagnostics.Trace.Flush();
        }
        else
        {
            ShowWindowAsync(runningProcesses[0].MainWindowHandle, 6);
            ShowWindowAsync(runningProcesses[0].MainWindowHandle, 9);
        }
    }

    static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
    {
        HandleUnhandledException(e);
    }

    static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        HandleUnhandledException(e);
    }

    static void HandleUnhandledException(Object o)
    {
        Exception e = o as Exception;

        // What happens if we cannot start?
        Trace.WriteLine(new LogMessage("Main", "Unhandled Exception: " + e.Message), LogType.Error.ToString());
        Trace.WriteLine(new LogMessage("Main", "Stack Trace: " + e.StackTrace), LogType.Error.ToString());
        Environment.Exit(1);

        // TODO: Can we just restart the application?

        // Shutdown the application
        Environment.Exit(1);
    }

    [DllImport("User32.dll")]
    public static extern int ShowWindowAsync(IntPtr hWnd, int swCommand);
}
}

