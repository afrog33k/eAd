using System;
using System.Collections.ObjectModel;
using System.Text;
using System.IO;
using Client.Properties;
using Client.Service;

namespace Client.Core
{
    class StatLog
    {
        private Collection<Stat> _stats;
        private ServiceClient _xmds;
        private String _lastSubmit;
        private HardwareKey _hardwareKey;
        private Boolean _xmdsProcessing;

        public StatLog()
        {
            _stats = new Collection<Stat>();
            _xmds = new ServiceClient();

            // Register a listener for the XMDS stats
            _xmds.SubmitStatsCompleted += (_xmds_SubmitStatsCompleted);

            // Get the key for this display
            _hardwareKey = new HardwareKey();

            _xmdsProcessing = false;
        }

        /// <summary>
        /// Record a complete Layout Event
        /// </summary>
        /// <param name="fromDT"></param>
        /// <param name="toDT"></param>
        /// <param name="scheduleID"></param>
        /// <param name="layoutID"></param>
        public void RecordLayout(String fromDT, String toDT, int scheduleID, int layoutID)
        {
            if (!Settings.Default.statsEnabled) return;

            Stat stat = new Stat();

            stat.FileType = StatType.Layout;
            stat.FromDate = fromDT;
            stat.ToDate = toDT;
            stat.ScheduleID = scheduleID;
            stat.LayoutID = layoutID;

            _stats.Add(stat);

            return;
        }

        /// <summary>
        /// Record a complete Media Event
        /// </summary>
        /// <param name="fromDT"></param>
        /// <param name="toDT"></param>
        /// <param name="layoutID"></param>
        /// <param name="mediaID"></param>
        public void RecordMedia(String fromDT, String toDT, int layoutID, String mediaID)
        {
            if (!Settings.Default.statsEnabled) return;

            Stat stat = new Stat();

            stat.FileType = StatType.Media;
            stat.FromDate = fromDT;
            stat.ToDate = toDT;
            stat.LayoutID = layoutID;
            stat.MediaID = mediaID;

            _stats.Add(stat);

            return;
        }

        /// <summary>
        /// Record a complete Event
        /// </summary>
        /// <param name="fromDT"></param>
        /// <param name="toDT"></param>
        /// <param name="tag"></param>
        public void RecordEvent(String fromDT, String toDT, String tag)
        {
            if (!Settings.Default.statsEnabled) return;

            Stat stat = new Stat();

            stat.FileType = StatType.Event;
            stat.FromDate = fromDT;
            stat.ToDate = toDT;
            stat.Tag = tag;

            _stats.Add(stat);

            return;
        }

        /// <summary>
        /// RecordStat
        /// </summary>
        /// <param name="stat"></param>
        public void RecordStat(Stat stat)
        {
            if (!Settings.Default.statsEnabled) return;

            System.Diagnostics.Debug.WriteLine(String.Format("Recording a Stat Record. Current Count = {0}", _stats.Count.ToString()), LogType.Audit.ToString());

            _stats.Add(stat);

            // At some point we will need to flush the stats
            if (_stats.Count >= Settings.Default.StatsFlushCount)
            {
                Flush();
            }

            return;
        }

        /// <summary>
        /// Flush the stats
        /// </summary>
        public void Flush()
        {
            System.Diagnostics.Debug.WriteLine(new LogMessage("Flush", String.Format("IN")), LogType.Audit.ToString());

            // Determine if there is anything to flush
            if (_stats.Count < 1 || _xmdsProcessing) return;

            int threshold = ((int)Settings.Default.collectInterval * 5);

            // Determine where we want to log.
            if (Settings.Default.XmdsLastConnection.AddSeconds(threshold) < DateTime.Now)
            {
                FlushToFile();
            }
            else
            {
                FlushToXmds();
            }

            System.Diagnostics.Debug.WriteLine(new LogMessage("Flush", String.Format("OUT")), LogType.Audit.ToString());
        }

        /// <summary>
        /// Send the Stat to a File
        /// </summary>
        private void FlushToFile()
        {
            System.Diagnostics.Debug.WriteLine(new LogMessage("FlushToFile", String.Format("IN")), LogType.Audit.ToString());

            // There is something to flush - we want to parse the collection adding to the TextWriter each time.
            try
            {
                // Open the Text Writer
                StreamWriter tw = new StreamWriter(File.Open(App.UserAppDataPath + "//" + Settings.Default.StatsLogFile, FileMode.Append, FileAccess.Write, FileShare.Read), Encoding.UTF8);

                try
                {
                    foreach (Stat stat in _stats)
                    {
                        tw.WriteLine(stat.ToString());
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(new LogMessage("FlushToFile", String.Format("Error writing stats line to file with exception {0}", ex.Message)), LogType.Error.ToString());
                }
                finally
                {
                    // Close the tw.
                    tw.Close();
                    tw.Dispose();
                }
            }
            catch (Exception ex)
            {
                // Log this exception
                System.Diagnostics.Trace.WriteLine(new LogMessage("FlushToFile", String.Format("Error writing stats to file with exception {0}", ex.Message)), LogType.Error.ToString());
            }
            finally
            {
                // Always clear the stats. If the file open failed for some reason then we dont want to try again
                _stats.Clear();
            }

            System.Diagnostics.Debug.WriteLine(new LogMessage("FlushToFile", String.Format("OUT")), LogType.Audit.ToString());
        }

        /// <summary>
        /// Send the Stats to XMDS
        /// </summary>
        private void FlushToXmds()
        {
            System.Diagnostics.Debug.WriteLine(new LogMessage("FlushToXmds", String.Format("IN")), LogType.Audit.ToString());

            String stats;

            stats = "<log>";

            // Load the Stats collection into a string
            try
            {
                foreach (Stat stat in _stats)
                {
                    stats += stat.ToString();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(new LogMessage("FlushToXmds", String.Format("Error converting stat to a string {0}", ex.Message)), LogType.Error.ToString());
            }

            stats += "</log>";

            // Store the stats as the last sent (so we have a record if it fails)
            _lastSubmit = stats;

            // Clear the stats collection
            _stats.Clear();
            
            // Submit the string to XMDS
            _xmdsProcessing = true;

            _xmds.SubmitStatsAsync(Settings.Default.Version, Settings.Default.ServerKey, _hardwareKey.Key, stats);

            // Log out
            System.Diagnostics.Debug.WriteLine(new LogMessage("FlushToXmds", String.Format("OUT")), LogType.Audit.ToString());
        }

        /// <summary>
        /// Capture the XMDS call and see if it went well
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _xmds_SubmitStatsCompleted(object sender, SubmitStatsCompletedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(new LogMessage("_xmds_SubmitStatsCompleted", String.Format("IN")), LogType.Audit.ToString());

            _xmdsProcessing = false;

            // Test if we succeeded or not
            if (e.Error != null)
            {
                // We had an error, log it.
                System.Diagnostics.Trace.WriteLine(new LogMessage("_xmds_SubmitStatsCompleted", String.Format("Error during Submit to XMDS {0}", e.Error.Message)), LogType.Error.ToString());

                // Dump the stats to a file instead
                if (!String.IsNullOrEmpty(_lastSubmit))
                {
                    try
                    {
                        // Open the Text Writer
                        StreamWriter tw = new StreamWriter(File.Open(App.UserAppDataPath + "//" + Settings.Default.StatsLogFile, FileMode.Append, FileAccess.Write, FileShare.Read), Encoding.UTF8);

                        try
                        {
                            tw.Write(_lastSubmit);
                        }
                        catch (Exception ex)
                        {
                            // Log this exception
                            System.Diagnostics.Trace.WriteLine(new LogMessage("_xmds_SubmitStatsCompleted", String.Format("Error writing stats to file with exception {0}", ex.Message)), LogType.Error.ToString());
                        }
                        finally
                        {
                            tw.Close();
                            tw.Dispose();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log this exception
                        System.Diagnostics.Trace.WriteLine(new LogMessage("_xmds_SubmitStatsCompleted", String.Format("Could not open the file with exception {0}", ex.Message)), LogType.Error.ToString());
                    }
                }
            }

            // Clear the last sumbit
            _lastSubmit = "";

            System.Diagnostics.Debug.WriteLine(new LogMessage("_xmds_SubmitStatsCompleted", String.Format("OUT")), LogType.Audit.ToString());
        }
    }
}
