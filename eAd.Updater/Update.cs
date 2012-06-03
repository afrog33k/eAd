using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Ionic.Zip;
using eAd.Utilities;

namespace eAd.Updater
{
//Procesing
//Remove previous download file
//Download file
//Unzip file contents to temp folder
//Remove files from destination folder present in temp folder
//Move unzipped files to destination folder
//Remove download file
//Remove temp folder


public partial class Update : Form,IReporter
{
    private readonly string _updateFolder = Application.StartupPath + @"\updates\";
    private string _url = "";
    private bool _called = true;
    private string _destinationFolder = "";
    private string _downloadFile = "";
    private string _postProcessCommand = "";
    private string _postProcessFile = "";
    private string _processToEnd = "";
    private string _tempDownloadFolder = "";

    public Update()
    {
        InitializeComponent();
        _instance = this;
    }

    public void SetLabel(Label label, string text)
    {
        if (label.InvokeRequired)
        {
            SetLabelCallback d = SetLabel;
            label.Invoke(d, new object[] { label, text });
        }
        else
        {
            label.Text = text;
            label.Refresh();
            Invalidate();
        }
    }

    private void Form1Load(object sender, EventArgs e)
    {
        Hide();

        if (_called)
        {
            WindowState = FormWindowState.Normal;
            Show();

            var bw = new BackgroundWorker();

            bw.DoWork -= BackgroundWorker;
            bw.DoWork += BackgroundWorker;
            bw.WorkerSupportsCancellation = true;
            bw.RunWorkerAsync();
        }
    }

    private void BackgroundWorker(object sender, DoWorkEventArgs e)
    {
        PreDownload();

        if (_called)
        {

            this.Invoke((MethodInvoker)delegate
            {
                WindowState = FormWindowState.Normal;
                Show();




                SetLabel(line1, "Stopping " + _processToEnd);
            });
            Thread.Sleep(1000);

            try
            {
                Process[] processes = Process.GetProcesses();

                foreach (Process process in processes)
                {
                    if (process.ProcessName == _processToEnd)
                    {
                        process.Kill();
                    }
                }
            }
            catch (Exception)
            {
            }


            Webdata.BytesDownloaded += Bytesdownloaded;
            Webdata.DownloadFromWeb(_url, _downloadFile, _tempDownloadFolder);

            SetLabel(line1, "Unzipping package...");
            Thread.Sleep(1000);
            UnZip(_tempDownloadFolder + _downloadFile, _tempDownloadFolder);
            SetLabel(line1, "Moving files... from: " +_tempDownloadFolder + " to: " +_destinationFolder);
            Thread.Sleep(1000);
            MoveFiles();
            SetLabel(line1, "Wrapping up...");
            Thread.Sleep(1000);
            WrapUp();
            if (_postProcessFile != "") PostDownload();
        }

        this.Invoke((MethodInvoker)Close);

    }

    public void  Report(string messsage)
    {
        SetLabel(line1, messsage);
    }


    static  Update  _instance;

    public static Update Instance
    {
        get
        {

            return _instance;
        }
    }


    private void UnpackCommandline()
    {
        string cmdLn = Environment.GetCommandLineArgs().Aggregate("", (current, arg) => current + arg);

        if (cmdLn.IndexOf('|') == -1)
        {
            _called = false;
            var info = new Info();
            info.ShowDialog();
            Close();
        }


        string[] tmpCmd = cmdLn.Split('|');

        for (int i = 1; i < tmpCmd.GetLength(0); i++)
        {
            if (tmpCmd[i] == "downloadFile") _downloadFile = tmpCmd[i + 1];
            if (tmpCmd[i] == "URL") _url = tmpCmd[i + 1];
            if (tmpCmd[i] == "destinationFolder") _destinationFolder = tmpCmd[i + 1];
            if (tmpCmd[i] == "processToEnd") _processToEnd = tmpCmd[i + 1];
            if (tmpCmd[i] == "postProcess") _postProcessFile = tmpCmd[i + 1];
            if (tmpCmd[i] == "command") _postProcessCommand += @" /" + tmpCmd[i + 1];
            i++;
        }
    }

    private static void UnZip(string file, string unZipTo)
    {
        try
        {
            // Specifying Console.Out here causes diagnostic msgs to be sent to the Console
            // In a WinForms or WPF or Web app, you could specify nothing, or an alternate
            // TextWriter to capture diagnostic messages.

            using (ZipFile zip = ZipFile.Read(file))
            {
                // This call to ExtractAll() assumes:
                //   - none of the entries are password-protected.
                //   - want to extract all entries to current working directory
                //   - none of the files in the zip already exist in the directory;
                //     if they do, the method will throw.
                zip.ExtractAll(unZipTo);
            }
        }
        catch (Exception)
        {
        }
    }


    private void PreDownload()
    {
        if (!Directory.Exists(_updateFolder)) Directory.CreateDirectory(_updateFolder);

        _tempDownloadFolder = _updateFolder + DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + @"\";

        if (Directory.Exists(_tempDownloadFolder))
        {
            Directory.Delete(_tempDownloadFolder, true);
        }

        Directory.CreateDirectory(_tempDownloadFolder);

        UnpackCommandline();
    }

    private void PostDownload()
    {
        var startInfo = new ProcessStartInfo();
        startInfo.FileName = _postProcessFile.Replace(".vshost", "");
        startInfo.UseShellExecute = false;
        //  startInfo.Arguments = _postProcessCommand;
        Process.Start(startInfo);
    }


    private void WrapUp()
    {
        try
        {

     
        if (Directory.Exists(_tempDownloadFolder))
        {
            Directory.Delete(_tempDownloadFolder, true);
        }  
        }
        catch (Exception)
        {
            
           
        }
    }


    private void MoveFiles()
    {
        //    var di = new DirectoryInfo(_tempDownloadFolder);

        CopyUtility.CopyAllFiles(_tempDownloadFolder, _destinationFolder,true,this);

        SetLabel(line1, "All files moved...");
        //FileInfo[] files = di.GetFiles();

        //foreach (FileInfo fi in files)
        //{
        //    if (fi.Name != _downloadFile)
        //        File.Copy(_tempDownloadFolder + fi.Name, _destinationFolder + fi.Name, true);
        //}
    }

    private void Bytesdownloaded(ByteArgs e)
    {
        this.Invoke(new ThreadStart(() =>
        {
            progressBar1.Maximum = e.total;

            if (progressBar1.Value + e.Downloaded <= progressBar1.Maximum)
            {
                progressBar1.Value += e.Downloaded;
                SetLabel(line1, "Downloading Update...");
                SetLabel(lblPercentage, (progressBar1.Value*100 / progressBar1.Maximum) + "%");
            }
            else
            {
                SetLabel(line1, "Download complete.");

                SetLabel(lblPercentage, (progressBar1.Value*100 / progressBar1.Maximum) + "%");        
              
            }

            progressBar1.Refresh();
            Invalidate();
        }));
    }

    #region Nested type: SetLabelCallback

    private delegate void SetLabelCallback(Label label, string text);

    #endregion
}
}