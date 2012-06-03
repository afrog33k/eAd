namespace ClientApp.Core
{
    using ClientApp;
    using System;
    using System.Windows.Forms;

    internal static class Options
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
          OptionForm mainForm = new global::ClientApp.OptionForm();
            Application.Run(mainForm);
        }
    }
}

