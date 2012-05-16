using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.ObjectModel;
using ClientApp.Properties;
using ClientApp.Service;

namespace ClientApp.Core
{
class ClientTraceListener : TraceListener
{
    private Collection<TraceMessage> _traceMessages;
    private String _logPath;
    private Boolean _xmdsProcessing;
    private ServiceClient _xmds;
    private String _lastSubmit;
    private HardwareKey _hardwareKey;

    public ClientTraceListener()
    {
        InitializeListener();
    }

    public ClientTraceListener(string r_strListenerName)
    : base(r_strListenerName)
    {
        InitializeListener() ;
    }

    private void InitializeListener()
    {
        // Make a new collection of TraceMessages
        _traceMessages = new Collection<TraceMessage>();
        _logPath = App.UserAppDataPath + @"/" + Settings.Default.logLocation;

        _xmdsProcessing = false;
        _xmds =new ServiceClient();

        // Register a listener for the XMDS stats
        _xmds.SubmitLogCompleted +=(XmdsSubmitLogCompleted);

        // Get the key for this display
        _hardwareKey = new HardwareKey();
    }

    private void AddToCollection(string message, string category)
    {
        TraceMessage traceMessage;

        traceMessage.Category = category;
        traceMessage.DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        traceMessage.Message = message;

        _traceMessages.Add(traceMessage);
    }

    private void FlushToFile()
    {
        if (_traceMessages.Count < 1) return;

        try
        {
            // Open the Text Writer
            StreamWriter tw = new StreamWriter(File.Open(_logPath, FileMode.Append, FileAccess.Write, FileShare.Read), Encoding.UTF8);

            String theMessage;

            foreach (TraceMessage message in _traceMessages)
            {
                String traceMsg = message.Message.ToString();

                theMessage = String.Format("<trace date=\"{0}\" category=\"{1}\">{2}</trace>", message.DateTime, message.Category, traceMsg);
                tw.WriteLine(theMessage);
            }

            // Close the tw.
            tw.Close();
            tw.Dispose();

            // Remove the messages we have just added
            _traceMessages.Clear();
        }
        catch
        {
            // What can we do?
        }
        finally
        {
            _traceMessages.Clear();
        }
    }

    /// <summary>
    /// Flush the log to XMDS
    /// </summary>
    private void FlushToXmds()
    {
        String log;

        log = "<log>";

        // Load the Stats collection into a string
        try
        {
            foreach (TraceMessage traceMessage in _traceMessages)
            {
                String traceMsg = traceMessage.Message.ToString();

                log += String.Format("<trace date=\"{0}\" category=\"{1}\">{2}</trace>", traceMessage.DateTime, traceMessage.Category, traceMsg);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLine(new LogMessage("FlushToXmds", String.Format("Error converting stat to a string {0}", ex.Message)), LogType.Error.ToString());
        }

        log += "</log>";

        // Store the stats as the last sent (so we have a record if it fails)
        _lastSubmit = log;

        // Clear the stats collection
        _traceMessages.Clear();

        // Submit the string to XMDS
        _xmdsProcessing = true;

        _xmds.SubmitLogAsync(Settings.Default.Version, Settings.Default.ServerKey, _hardwareKey.Key, log);
    }

    /// <summary>
    /// Capture the XMDS call and see if it went well
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void XmdsSubmitLogCompleted(object sender, SubmitLogCompletedEventArgs e)
    {
        _xmdsProcessing = false;

        // Test if we succeeded or not
        if (e.Error != null)
        {
            // We had an error, log it.
            System.Diagnostics.Trace.WriteLine(new LogMessage("_xmds_SubmitLogCompleted", String.Format("Error during Submit to XMDS {0}", e.Error.Message)), LogType.Error.ToString());

            // Dump the stats to a file instead
            if (!String.IsNullOrEmpty(_lastSubmit))
            {
                try
                {
                    // Open the Text Writer
                    StreamWriter tw = new StreamWriter(File.Open(_logPath, FileMode.Append, FileAccess.Write, FileShare.Read), Encoding.UTF8);

                    try
                    {
                        tw.Write(_lastSubmit);
                    }
                    catch {}
                    finally
                    {
                        tw.Close();
                        tw.Dispose();
                    }
                }
                catch (Exception)
                {}
            }
        }

        // Clear the last sumbit
        _lastSubmit = "";
    }

    public override void Write(string message)
    {
        AddToCollection(message, "Audit");
    }

    public override void Write(object o)
    {
        AddToCollection(o.ToString(), "Audit");
    }

    public override void Write(string message, string category)
    {
        AddToCollection(message, category);
    }

    public override void Write(object o, string category)
    {
        AddToCollection(o.ToString(), category);
    }

    public override void WriteLine(string message)
    {
        Write(message + "\n");
    }

    public override void WriteLine(object o)
    {
        Write(o.ToString() + "\n");
    }

    public override void WriteLine(string message, string category)
    {
        Write((message + "\n"), category);
    }

    public override void WriteLine(object o, string category)
    {
        Write((o.ToString() + "\n"), category);
    }

    public override void Fail(string message)
    {
        StackTrace objTrace = new StackTrace(true);
        message += "\n" + objTrace.ToString();

        AddToCollection(message, "");
    }

    public override void Fail(string message, string detailMessage)
    {
        StackTrace objTrace = new StackTrace(true);
        message += "\n" + objTrace.ToString();

        AddToCollection(message, detailMessage);
    }

    public override void Close()
    {
        // Determine if there is anything to flush
        if (_traceMessages.Count < 1) return;

        // As we are closing if XMDS is already busy just log to file.
        if (_xmdsProcessing)
        {
            FlushToFile();
        }
        else
        {
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
        }
    }

    public override void Flush()
    {
        // Determine if there is anything to flush
        if (_traceMessages.Count < 1 || _xmdsProcessing) return;

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
    }
}
}
