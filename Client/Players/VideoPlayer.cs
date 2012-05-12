

using System.Windows;
using System.Windows.Forms.Integration;
using AxWMPLib;

namespace Client
{
public sealed partial class VideoPlayer : Media
{
    public VideoPlayer() : base(0,0,0,0)
    {
        InitializeComponent();


        this.TopLevel = false;

        _finished = false;
        var host = new WindowsFormsHost();

        var resources = new System.ComponentModel.ComponentResourceManager(typeof(VideoPlayer));
        this.axWindowsMediaPlayer1 = new AxWMPLib.AxWindowsMediaPlayer();
        ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).BeginInit();

        //
        axWindowsMediaPlayer1.PlayStateChange +=new AxWMPLib._WMPOCXEvents_PlayStateChangeEventHandler(axWindowsMediaPlayer1_PlayStateChange);
        //
        this.axWindowsMediaPlayer1.Enabled = true;
        this.axWindowsMediaPlayer1.Location = new System.Drawing.Point(0, 0);
        this.axWindowsMediaPlayer1.Name = "axWindowsMediaPlayer1";
        this.axWindowsMediaPlayer1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axWindowsMediaPlayer1.OcxState")));
        this.axWindowsMediaPlayer1.Size = new System.Drawing.Size(291, 269);
        this.axWindowsMediaPlayer1.TabIndex = 0;

        //
        // VideoPlayer
        //
        this.AutoScaleDimensions = new Size(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new Size(292, 266);

        host.Child =axWindowsMediaPlayer1;
        HasOnLoaded = true;
        MediaCanvas.Children.Add(host);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        this.Name = "VideoPlayer";
        this.Opacity = 0.8;
        this.Text = "VideoPlayer";
        ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).EndInit();

    }

    private void axWindowsMediaPlayer1_PlayStateChange(object sender, _WMPOCXEvents_PlayStateChangeEvent e)
    {
        if(e.newState == 3) // Werid
        {
            OnLoaded();
        }
    }

    private bool TopLevel
    {
        get;
        set;
    }

    public void StartPlayer(string filePath)
    {
        axWindowsMediaPlayer1.Visible = true;
        axWindowsMediaPlayer1.Width = (int) this.Width;
        axWindowsMediaPlayer1.Height = (int) this.Height;
        axWindowsMediaPlayer1.Location = new System.Drawing.Point(0, 0);

        axWindowsMediaPlayer1.uiMode = "none";
        axWindowsMediaPlayer1.URL = filePath;
        axWindowsMediaPlayer1.stretchToFit = true;
        axWindowsMediaPlayer1.windowlessVideo = true;

        axWindowsMediaPlayer1.PlayStateChange += new AxWMPLib._WMPOCXEvents_PlayStateChangeEventHandler(AxWmpPlayStateChange);
    }

    void AxWmpPlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
    {
        if (e.newState == 8)
        {
            // indicate we are stopped
            _finished = true;
        }
    }

    /// <summary>
    /// Has this player finished playing
    /// </summary>
    public bool FinishedPlaying
    {
        get
        {
            return this._finished;
        }
    }




    private bool _finished;


}
}