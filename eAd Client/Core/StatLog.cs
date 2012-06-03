

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

    public class StatLog
    {
        private HardwareKey _hardwareKey;
        private string _lastSubmit;
        private Collection<Stat> _stats = new Collection<Stat>();
        private ServiceClient _xmds = new ServiceClient();
        private bool _xmdsProcessing;

        public StatLog()
        {
            this._xmds.SubmitStatsCompleted += new EventHandler<SubmitStatsCompletedEventArgs>(this._xmds_SubmitStatsCompleted);
            this._hardwareKey = new HardwareKey();
            this._xmdsProcessing = false;
        }

        private void _xmds_SubmitStatsCompleted(object sender, SubmitStatsCompletedEventArgs e)
        {
            this._xmdsProcessing = false;
            if (e.Error != null)
            {
                Trace.WriteLine(new LogMessage("_xmds_SubmitStatsCompleted", string.Format("Error during Submit to XMDS {0}", e.Error.Message)), LogType.Error.ToString());
                if (!string.IsNullOrEmpty(this._lastSubmit))
                {
                    try
                    {
                        StreamWriter writer = new StreamWriter(File.Open(App.UserAppDataPath + "//" + Settings.Default.StatsLogFile, FileMode.Append, FileAccess.Write, FileShare.Read), Encoding.UTF8);
                        try
                        {
                            writer.Write(this._lastSubmit);
                        }
                        catch (Exception exception)
                        {
                            Trace.WriteLine(new LogMessage("_xmds_SubmitStatsCompleted", string.Format("Error writing stats to file with exception {0}", exception.Message)), LogType.Error.ToString());
                        }
                        finally
                        {
                            writer.Close();
                            writer.Dispose();
                        }
                    }
                    catch (Exception exception2)
                    {
                        Trace.WriteLine(new LogMessage("_xmds_SubmitStatsCompleted", string.Format("Could not open the file with exception {0}", exception2.Message)), LogType.Error.ToString());
                    }
                }
            }
            this._lastSubmit = "";
        }

        public void Flush()
        {
            if ((this._stats.Count >= 1) && !this._xmdsProcessing)
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
            try
            {
                StreamWriter writer = new StreamWriter(File.Open(App.UserAppDataPath + "//" + Settings.Default.StatsLogFile, FileMode.Append, FileAccess.Write, FileShare.Read), Encoding.UTF8);
                try
                {
                    foreach (Stat stat in this._stats)
                    {
                        writer.WriteLine(stat.ToString());
                    }
                }
                catch (Exception exception)
                {
                    Trace.WriteLine(new LogMessage("FlushToFile", string.Format("Error writing stats line to file with exception {0}", exception.Message)), LogType.Error.ToString());
                }
                finally
                {
                    writer.Close();
                    writer.Dispose();
                }
            }
            catch (Exception exception2)
            {
                Trace.WriteLine(new LogMessage("FlushToFile", string.Format("Error writing stats to file with exception {0}", exception2.Message)), LogType.Error.ToString());
            }
            finally
            {
                this._stats.Clear();
            }
        }

        private void FlushToXmds()
        {
            string statXml = "<log>";
            try
            {
                foreach (Stat stat in this._stats)
                {
                    statXml = statXml + stat.ToString();
                }
            }
            catch (Exception exception)
            {
                Trace.WriteLine(new LogMessage("FlushToXmds", string.Format("Error converting stat to a string {0}", exception.Message)), LogType.Error.ToString());
            }
            statXml = statXml + "</log>";
            this._lastSubmit = statXml;
            this._stats.Clear();
            this._xmdsProcessing = true;
            this._xmds.SubmitStatsAsync(Settings.Default.Version, Settings.Default.ServerKey, this._hardwareKey.Key, statXml);
        }

        public void RecordEvent(string fromDT, string toDT, string tag)
        {
            if (Settings.Default.statsEnabled)
            {
                Stat item = new Stat {
                    FileType = StatType.Event,
                    FromDate = fromDT,
                    ToDate = toDT,
                    Tag = tag
                };
                this._stats.Add(item);
            }
        }

        public void RecordLayout(string fromDT, string toDT, int scheduleID, int layoutID)
        {
            if (Settings.Default.statsEnabled)
            {
                Stat item = new Stat {
                    FileType = StatType.Layout,
                    FromDate = fromDT,
                    ToDate = toDT,
                    ScheduleID = scheduleID,
                    LayoutID = layoutID
                };
                this._stats.Add(item);
            }
        }

        public void RecordMedia(string fromDT, string toDT, int layoutID, string mediaID)
        {
            if (Settings.Default.statsEnabled)
            {
                Stat item = new Stat {
                    FileType = StatType.Media,
                    FromDate = fromDT,
                    ToDate = toDT,
                    LayoutID = layoutID,
                    MediaID = mediaID
                };
                this._stats.Add(item);
            }
        }

        public void RecordStat(Stat stat)
        {
            if (Settings.Default.statsEnabled)
            {
                this._stats.Add(stat);
                if (this._stats.Count >= Settings.Default.StatsFlushCount)
                {
                    this.Flush();
                }
            }
        }
    }
}

