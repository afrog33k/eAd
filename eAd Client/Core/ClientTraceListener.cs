using ClientApp.Service;

namespace ClientApp.Core
{
    using ClientApp;
    using ClientApp.Properties;
    
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Text;

    internal class ClientTraceListener : TraceListener
    {
        private HardwareKey _hardwareKey;
        private string _lastSubmit;
        private string _logPath;
        private Collection<TraceMessage> _traceMessages;
        private ServiceClient _xmds;
        private bool _xmdsProcessing;

        public ClientTraceListener()
        {
            this.InitializeListener();
        }

        public ClientTraceListener(string r_strListenerName) : base(r_strListenerName)
        {
            this.InitializeListener();
        }

        private void AddToCollection(string message, string category)
        {
            TraceMessage message2;
            message2.Category = category;
            message2.DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            message2.Message = message;
            this._traceMessages.Add(message2);
        }

        public override void Close()
        {
            if (this._traceMessages.Count >= 1)
            {
                if (this._xmdsProcessing)
                {
                    this.FlushToFile();
                }
                else
                {
                    int num = ((int) Settings.Default.collectInterval) * 5;
                    if (Settings.Default.XmdsLastConnection.AddSeconds((double) num) < DateTime.Now)
                    {
                        this.FlushToFile();
                    }
                    else
                    {
                        this.FlushToXmds();
                    }
                }
            }
        }

        public override void Fail(string message)
        {
            message = message + "\n" + new StackTrace(true).ToString();
            this.AddToCollection(message, "");
        }

        public override void Fail(string message, string detailMessage)
        {
            message = message + "\n" + new StackTrace(true).ToString();
            this.AddToCollection(message, detailMessage);
        }

        public override void Flush()
        {
            if ((this._traceMessages.Count >= 1) && !this._xmdsProcessing)
            {
                int num = ((int) Settings.Default.collectInterval) * 5;
                if (Settings.Default.XmdsLastConnection.AddSeconds((double) num) < DateTime.Now)
                {
                    this.FlushToFile();
                }
                else
                {
                    this.FlushToXmds();
                }
            }
        }

        private void FlushToFile()
        {
            if (this._traceMessages.Count >= 1)
            {
                try
                {
                    StreamWriter writer = new StreamWriter(File.Open(this._logPath, FileMode.Append, FileAccess.Write, FileShare.Read), Encoding.UTF8);
                    foreach (TraceMessage message in this._traceMessages)
                    {
                        string str2 = message.Message.ToString();
                        string str = string.Format("<trace date=\"{0}\" category=\"{1}\">{2}</trace>", message.DateTime, message.Category, str2);
                        writer.WriteLine(str);
                    }
                    writer.Close();
                    writer.Dispose();
                    this._traceMessages.Clear();
                }
                catch
                {
                }
                finally
                {
                    this._traceMessages.Clear();
                }
            }
        }

        private void FlushToXmds()
        {
            string logXml = "<log>";
            try
            {
                foreach (TraceMessage message in this._traceMessages)
                {
                    string str2 = message.Message.ToString();
                    logXml = logXml + string.Format("<trace date=\"{0}\" category=\"{1}\">{2}</trace>", message.DateTime, message.Category, str2);
                }
            }
            catch (Exception exception)
            {
                Trace.WriteLine(new LogMessage("FlushToXmds", string.Format("Error converting stat to a string {0}", exception.Message)), LogType.Error.ToString());
            }
            logXml = logXml + "</log>";
            this._lastSubmit = logXml;
            this._traceMessages.Clear();
            this._xmdsProcessing = true;
            this._xmds.SubmitLogAsync(Settings.Default.Version, Settings.Default.ServerKey, this._hardwareKey.Key, logXml);
        }

        private void InitializeListener()
        {
            this._traceMessages = new Collection<TraceMessage>();
            this._logPath = App.UserAppDataPath + "/" + Settings.Default.logLocation;
            this._xmdsProcessing = false;
            this._xmds = new ServiceClient();
            this._xmds.SubmitLogCompleted += new EventHandler<SubmitLogCompletedEventArgs>(this.XmdsSubmitLogCompleted);
            this._hardwareKey = new HardwareKey();
        }

        public override void Write(object o)
        {
            this.AddToCollection(o.ToString(), "Audit");
        }

        public override void Write(string message)
        {
            this.AddToCollection(message, "Audit");
        }

        public override void Write(object o, string category)
        {
            this.AddToCollection(o.ToString(), category);
        }

        public override void Write(string message, string category)
        {
            this.AddToCollection(message, category);
        }

        public override void WriteLine(object o)
        {
            this.Write(o.ToString() + "\n");
        }

        public override void WriteLine(string message)
        {
            this.Write(message + "\n");
        }

        public override void WriteLine(object o, string category)
        {
            this.Write(o.ToString() + "\n", category);
        }

        public override void WriteLine(string message, string category)
        {
            this.Write(message + "\n", category);
        }

        private void XmdsSubmitLogCompleted(object sender, SubmitLogCompletedEventArgs e)
        {
            this._xmdsProcessing = false;
            if (e.Error != null)
            {
                Trace.WriteLine(new LogMessage("_xmds_SubmitLogCompleted", string.Format("Error during Submit to XMDS {0}", e.Error.Message)), LogType.Error.ToString());
                if (!string.IsNullOrEmpty(this._lastSubmit))
                {
                    try
                    {
                        StreamWriter writer = new StreamWriter(File.Open(this._logPath, FileMode.Append, FileAccess.Write, FileShare.Read), Encoding.UTF8);
                        try
                        {
                            writer.Write(this._lastSubmit);
                        }
                        catch
                        {
                        }
                        finally
                        {
                            writer.Close();
                            writer.Dispose();
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            this._lastSubmit = "";
        }
    }
}

