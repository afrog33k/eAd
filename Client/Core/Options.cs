using System;
using System.Windows.Forms;

namespace ClientApp.Core
{
static class Options
{
    /// <summary>
    /// The main entry point for the options.
    /// </summary>
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        OptionForm formOptions = new OptionForm();
        Application.Run(formOptions);
    }
}
}