using System;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.Diagnostics;
using System.Windows.Forms;
using ClientApp.Core;
using ClientApp.Properties;
using ClientApp.Service;

namespace ClientApp
{
public partial class OptionForm : Form
{
    private HardwareKey _hardwareKey;


    public OptionForm()
    {
        InitializeComponent();

        System.Diagnostics.Debug.WriteLine("Initialise Option Form Components", "OptionForm");

        // Get a hardware key here, just in case we havent been able to get one before
        _hardwareKey = new HardwareKey();

        // XMDS completed event
        xmds.RegisterDisplayCompleted += (Xmds1RegisterDisplayCompleted);

        // Library Path
        if (Settings.Default.LibraryPath == "DEFAULT")
        {
            Debug.WriteLine("Getting the Library Path", "OptionForm");
            Settings.Default.LibraryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ Library";
            Settings.Default.Save();
        }

        // Computer name if the display name hasnt been set yet
        if (Settings.Default.displayName == "COMPUTERNAME")
        {
            Debug.WriteLine("Getting the display Name", "OptionForm");
            Settings.Default.displayName = Environment.MachineName;
            Settings.Default.Save();
        }

        // Set global proxy information
        OptionForm.SetGlobalProxy();

        // Settings Tab
        textBoxXmdsUri.Text = Settings.Default.serverURI;
        textBoxServerKey.Text = Settings.Default.ServerKey;
        textBoxLibraryPath.Text = Settings.Default.LibraryPath;
        tbHardwareKey.Text = Settings.Default.hardwareKey;
        numericUpDownCollect.Value = Settings.Default.collectInterval;
        checkBoxPowerPoint.Checked = Settings.Default.powerpointEnabled;
        checkBoxStats.Checked = Settings.Default.statsEnabled;
        nupScrollStepAmount.Value = Settings.Default.scrollStepAmount;

        // Register Tab
        labelXmdsUrl.Text = Settings.Default.Xmds;
        textBoxDisplayName.Text = Settings.Default.displayName;

        // Proxy Tab
        textBoxProxyUser.Text = Settings.Default.ProxyUser;
        maskedTextBoxProxyPass.Text = Settings.Default.ProxyPassword;
        textBoxProxyDomain.Text = Settings.Default.ProxyDomain;

        // Client Tab
        clientWidth.Value = Settings.Default.sizeX;
        clientHeight.Value = Settings.Default.sizeY;
        offsetX.Value = Settings.Default.offsetX;
        offsetY.Value = Settings.Default.offsetY;

        // Advanced Tab
        numericUpDownEmptyRegions.Value = Settings.Default.emptyLayoutDuration;
        cbExpireModifiedLayouts.Checked = Settings.Default.expireModifiedLayouts;
        enableMouseCb.Checked = Settings.Default.EnableMouse;
        doubleBufferingCheckBox.Checked = Settings.Default.DoubleBuffering;

        System.Diagnostics.Debug.WriteLine("Loaded Options Form", "OptionForm");
    }

    /// <summary>
    /// Register display completed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void Xmds1RegisterDisplayCompleted(object sender,RegisterDisplayCompletedEventArgs e)
    {
        if (e.Error != null)
        {
            textBoxResults.Text = e.Error.Message;

            System.Diagnostics.Debug.WriteLine("Error returned from Call to XMDS Register Display.", "xmds1_RegisterDisplayCompleted");
            System.Diagnostics.Debug.WriteLine(e.Error.Message, "xmds1_RegisterDisplayCompleted");
            System.Diagnostics.Debug.WriteLine(e.Error.StackTrace, "xmds1_RegisterDisplayCompleted");
        }
        else
        {
            textBoxResults.Text = e.Result;
        }
    }

    /// <summary>
    /// Register display clicked
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void buttonRegister_Click(object sender, EventArgs e)
    {
        // Make a new hardware key just in case we have changed it in the form.
        _hardwareKey = new HardwareKey();

        textBoxResults.Text = "Sending Request";

        Settings.Default.Xmds = textBoxXmdsUri.Text.TrimEnd('/') + @"/data.asmx";
        //   xmds.Url = Settings.Default.Xmds;

        Settings.Default.displayName = textBoxDisplayName.Text;
        Settings.Default.Save();

        xmds.RegisterDisplayAsync(Settings.Default.ServerKey, _hardwareKey.Key, textBoxDisplayName.Text, Settings.Default.Version);
    }

    /// <summary>
    /// Save settings
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void buttonSaveSettings_Click(object sender, EventArgs e)
    {
        try
        {
            // Simple settings
            Settings.Default.ServerKey = textBoxServerKey.Text;
            Settings.Default.LibraryPath = textBoxLibraryPath.Text.TrimEnd('\\');
            Settings.Default.serverURI = textBoxXmdsUri.Text;
            Settings.Default.collectInterval = numericUpDownCollect.Value;
            Settings.Default.powerpointEnabled = checkBoxPowerPoint.Checked;
            Settings.Default.statsEnabled = checkBoxStats.Checked;
            Settings.Default.Xmds = textBoxXmdsUri.Text.TrimEnd('/') + @"/data.asmx";
            Settings.Default.hardwareKey = tbHardwareKey.Text;
            Settings.Default.scrollStepAmount = nupScrollStepAmount.Value;
            Settings.Default.EnableMouse = enableMouseCb.Checked;
            Settings.Default.DoubleBuffering = doubleBufferingCheckBox.Checked;

            // Also tweak the address of the xmds1
            //    xmds.Url = Settings.Default.Xmds;
            labelXmdsUrl.Text = Settings.Default.Xmds;

            // Proxy Settings
            Settings.Default.ProxyUser = textBoxProxyUser.Text;
            Settings.Default.ProxyPassword = maskedTextBoxProxyPass.Text;
            Settings.Default.ProxyDomain = textBoxProxyDomain.Text;

            // Change the default Proxy class
            OptionForm.SetGlobalProxy();

            // Client settings
            Settings.Default.sizeX = clientWidth.Value;
            Settings.Default.sizeY = clientHeight.Value;
            Settings.Default.offsetX = offsetX.Value;
            Settings.Default.offsetY = offsetY.Value;

            // Advanced settings
            Settings.Default.expireModifiedLayouts = cbExpireModifiedLayouts.Checked;
            Settings.Default.emptyLayoutDuration = numericUpDownEmptyRegions.Value;

            // Commit these changes back to the user settings
            Settings.Default.Save();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
        this.Close();
    }


    private void buttonLibrary_Click(object sender, EventArgs e)
    {
        // Set the dialog
        folderBrowserLibrary.SelectedPath = textBoxLibraryPath.Text;

        // Open the dialog
        if (folderBrowserLibrary.ShowDialog() == DialogResult.OK)
        {
            textBoxLibraryPath.Text = folderBrowserLibrary.SelectedPath;
        }
    }

    private void OnlineHelpToolStripMenuItemClick(object sender, EventArgs e)
    {
        // open URL in separate instance of default browser
        try
        {
            System.Diagnostics.Process.Start("http://1.9.13.61/Manual");
        }
        catch
        {
            MessageBox.Show("No web browser installed");
        }
    }

    private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
    {
        About about = new About();
        about.ShowDialog();
    }

    private void buttonDisplayAdmin_Click(object sender, EventArgs e)
    {
        // open URL in separate instance of default browser
        try
        {
            System.Diagnostics.Process.Start(Settings.Default.serverURI + @"/index.php?p=display");
        }
        catch
        {
            MessageBox.Show("No web browser installed");
        }
    }

    /// <summary>
    /// Sets up the global proxy
    /// </summary>
    public static void SetGlobalProxy()
    {
        Debug.WriteLine("[IN]", "SetGlobalProxy");

        Debug.WriteLine("Trying to detect a proxy.", "SetGlobalProxy");

        if (!String.IsNullOrEmpty(Settings.Default.ProxyUser))
        {
            // disable expect100Continue
            ServicePointManager.Expect100Continue = false;

            Debug.WriteLine("Creating a network credential using the Proxy User.", "SetGlobalProxy");

            NetworkCredential nc = new NetworkCredential(Settings.Default.ProxyUser, Settings.Default.ProxyPassword);

            if (!String.IsNullOrEmpty(Settings.Default.ProxyDomain))
                nc.Domain = Settings.Default.ProxyDomain;

            WebRequest.DefaultWebProxy.Credentials = nc;
        }
        else
        {
            Debug.WriteLine("No Proxy.", "SetGlobalProxy");
            WebRequest.DefaultWebProxy.Credentials = null;
        }

        // What if the URL for XMDS has a SSL certificate?
        ServicePointManager.ServerCertificateValidationCallback += delegate(object sender, X509Certificate certificate, X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            Debug.WriteLine("[IN]", "ServerCertificateValidationCallback");
            bool validationResult = false;

            Debug.WriteLine(certificate.Subject);
            Debug.WriteLine(certificate.Issuer);

            if (sslPolicyErrors != System.Net.Security.SslPolicyErrors.None)
            {
                Debug.WriteLine(sslPolicyErrors.ToString());
            }

            validationResult = true;

            Debug.WriteLine("[OUT]", "ServerCertificateValidationCallback");
            return validationResult;
        };

        Debug.WriteLine("[OUT]", "SetGlobalProxy");

        return;
    }
}
}