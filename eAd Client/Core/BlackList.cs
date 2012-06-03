using ClientApp.Service;

namespace ClientApp.Core
{
    using ClientApp;
    using ClientApp.Properties;
    
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Text;

    public class BlackList : IDisposable
    {
        private string blackListFile = (App.UserAppDataPath + "//" + Settings.Default.blackListLocation);
        private bool disposed;
        private HardwareKey hardwareKey = new HardwareKey();
        private ServiceClient xmds1;

        public void Add(List<string> items)
        {
            Trace.WriteLine(new LogMessage("Blacklist - Add", "Adding XMLNodeList to Blacklist"), LogType.Info.ToString());
            foreach (string str in items)
            {
                this.AddLocal(str);
            }
        }

        public void Add(string id, BlackListType type, string reason)
        {
            int num;
            if (reason == "")
            {
                reason = "No reason provided";
            }
            if (!int.TryParse(id, out num))
            {
                Trace.WriteLine(string.Format("Currently can only append Integer CurrentMedia types. Id {0}", id), "BlackList - Add");
            }
            this.xmds1 = new ServiceClient();
            this.xmds1.BlackListCompleted += new EventHandler<AsyncCompletedEventArgs>(this.xmds1_BlackListCompleted);
            this.xmds1.BlackListAsync(Settings.Default.ServerKey, this.hardwareKey.Key, num, type.ToString(), reason, Settings.Default.Version);
            this.AddLocal(id);
        }

        private void AddLocal(string id)
        {
            try
            {
                StreamWriter writer = new StreamWriter(File.Open(this.blackListFile, FileMode.Append, FileAccess.Write, FileShare.Read), Encoding.UTF8);
                writer.Write(string.Format("[{0}],", id));
                writer.Close();
            }
            catch (Exception)
            {
                Trace.WriteLine(string.Format("Cant add {0} to the blacklist", id));
            }
        }

        public bool BlackListed(string fileId)
        {
            StreamReader reader = null;
            if (File.Exists(this.blackListFile))
            {
                try
                {
                    reader = new StreamReader(File.Open(this.blackListFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                    return reader.ReadToEnd().Contains(string.Format("[{0}]", fileId));
                }
                catch (Exception)
                {
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }
                }
            }
            return false;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
            }
            this.disposed = true;
        }

        public void Truncate()
        {
            try
            {
                File.Delete(App.UserAppDataPath + "//" + Settings.Default.blackListLocation);
            }
            catch (Exception exception)
            {
                Trace.WriteLine("Cannot truncate the BlackList", "Blacklist - Truncate");
                Trace.WriteLine(exception.Message);
            }
        }

        private void xmds1_BlackListCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Trace.WriteLine("Error sending blacklist", "BlackList - BlackListCompleted");
            }
            else
            {
                Trace.WriteLine("Blacklist sending complete", "BlackList - BlackListCompleted");
            }
        }
    }
}

