
using System;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Navigation;
using System.Windows.Threading;
using Client.Core;
using Client.Properties;

namespace Client
{
class Text : Media
{
    private string _filePath;
    private string _direction;
    private string _backgroundImage;
    private string _backgroundColor;
    private WebBrowser _webBrowser;
    private string _documentText;
    private String _headText;
    private String _headJavaScript;

    private string _backgroundTop;
    private string _backgroundLeft;
    private double _scaleFactor;
    private int _scrollSpeed;

    private TemporaryHtml _tempHtml;

    /// <summary>
    /// Creates a Text display control
    /// </summary>
    /// <param name="options">Region Options for this control</param>
    public Text(RegionOptions options)
    : base(options.Width, options.Height, options.Top, options.Left)
    {
        // Collect some options from the Region Options passed in
        // and store them in member variables.
        _filePath = options.Uri;
        _direction = options.direction;
        _backgroundImage = options.backgroundImage;
        _backgroundColor = options.backgroundColor;
        _scaleFactor = options.ScaleFactor;
        _backgroundTop = options.BackgroundTop + "px";
        _backgroundLeft = options.BackgroundLeft + "px";
        _documentText = options.text;
        _scrollSpeed = options.scrollSpeed;
        _headJavaScript = options.javaScript;

        // Generate a temporary file to store the rendered object in.
        _tempHtml = new TemporaryHtml();

        // Generate the Head Html and store to file.
        GenerateHeadHtml();

        // Generate the Body Html and store to file.
        GenerateBodyHtml();

        // Fire up a webBrowser control to display the completed file.
        _webBrowser = new WebBrowser();
        _webBrowser.Height = options.Height;
        _webBrowser.Width = options.Width;
        _webBrowser.Margin = new Thickness(0,0,0,0);
        //_webBrowser.ScrollBarsEnabled = false;
        //_webBrowser.ScriptErrorsSuppressed = true;
        _webBrowser.LoadCompleted +=  (WebBrowserDocumentCompleted);

        _webBrowser.Navigated += delegate
        {
            HideScriptErrors(_webBrowser, true);
        };
        // Navigate to temp file
        _webBrowser.Navigate(_tempHtml.Path);

        MediaCanvas.Children.Add(_webBrowser);
    }

    public void HideScriptErrors(WebBrowser wb, bool hide)
    {
        FieldInfo fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
        if (fiComWebBrowser == null) return;
        object objComWebBrowser = fiComWebBrowser.GetValue(wb);
        if (objComWebBrowser == null) return;
        objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { hide });
    }

    #region Members

    /// <summary>
    /// Generates the Body Html for this Document
    /// </summary>
    private void GenerateBodyHtml()
    {
        String startPosition = "left";

        if (_direction == "right")
            startPosition = "right";

        // Generate the Body
        if (_direction == "none")
        {
            // Just use the RAW text that was in the XLF
            _tempHtml.BodyContent = _documentText;
        }
        else
        {
            // Format the text in some way
            String textRender = "";
            String textWrap = "";

            if (_direction == "left" || _direction == "right") textWrap = "white-space: nowrap";

            textRender += string.Format("<div id='text' style='position:relative;overflow:hidden;Width:{0}px; Height:{1}px;'>", this.Width - 10, this.Height);
            textRender += string.Format("<div id='innerText' style='position:absolute; {3}: 0px; top: 0px; Width:{2}px; {0}'>{1}</div></div>", textWrap, _documentText, this.Width - 10, startPosition);

            _tempHtml.BodyContent = textRender;
        }
    }

    /// <summary>
    /// Generates the Head Html for this Document
    /// </summary>
    private void GenerateHeadHtml()
    {
        // Handle the background
        String bodyStyle;

        if (_backgroundImage == null || _backgroundImage == "")
        {
            bodyStyle = "background-color:" + _backgroundColor + " ;";
        }
        else
        {
            bodyStyle = "background-image: url('" + _backgroundImage + "'); background-attachment:fixed; background-color:" + _backgroundColor + " background-repeat: no-repeat; background-position: " + _backgroundLeft + " " + _backgroundTop + ";";
        }

        // Do we need to include the init function to kick off the text render?
        String initFunction = "";

        if (_direction != "none")
        {
            initFunction = @"
<script type='text/javascript'>
function init() 
{ 
    tr = new TextRender('text', 'innerText', '" + _direction + @"', " + Settings.Default.scrollStepAmount.ToString() + @");

    var timer = 0;
    timer = setInterval('tr.TimerTick()', " + _scrollSpeed.ToString() + @");
}
</script>";
        }

        _headText = _headJavaScript + initFunction + "<style type='text/css'>body {" + bodyStyle + " font-size:" + _scaleFactor.ToString() + "em; }</style>";

        // Store the document text in the temporary HTML space
        _tempHtml.HeadContent = _headText;
    }

    /// <summary>
    /// Render media
    /// </summary>
    public override void RenderMedia()
    {
        base.StartTimer();
    }

    #endregion

    #region Event Handlers

    void WebBrowserDocumentCompleted(object sender, NavigationEventArgs navigationEventArgs)
    {
        // We have navigated to the temporary file.
        Show();
        //     MediaCanvas.Children.Add(_webBrowser);

        App.DoEvents();
    }

    #endregion

    /// <summary>
    /// Dispose of this text item
    /// </summary>
    /// <param name="disposing"></param>
    public override void Dispose()
    {

        {
            // Remove the webbrowser control
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(()=>
                {
                    MediaCanvas.Children.Remove(_webBrowser);
                    _webBrowser.Dispose();
                }));
            }
            catch
            {
                System.Diagnostics.Trace.WriteLine(new LogMessage("WebBrowser still in use.", String.Format("Dispose")));
            }

            // Remove the temporary file we created
            try
            {
                _tempHtml.Dispose();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(new LogMessage("Dispose", String.Format("Unable to dispose TemporaryHtml with exception {0}", ex.Message)));
            }
        }

        base.Dispose();
    }
}
}
