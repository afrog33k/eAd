// Type: AxWMPLib.AxWindowsMediaPlayer
// Assembly: AxInterop.WMPLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\ProjectHub\eAd\Client\Libraries\AxInterop.WMPLib.dll

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WMPLib;

namespace AxWMPLib
{
    [DesignTimeVisible(true)]
    [AxHost.ClsidAttribute("{6bf52a52-394a-11d3-b153-00c04f79faa6}")]
    public class AxWindowsMediaPlayer : AxHost
    {
        [DispId(1)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual string URL { get; set; }

        [DispId(2)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual WMPOpenState openState { get; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DispId(10)]
        public virtual WMPPlayState playState { get; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DispId(4)]
        public virtual IWMPControls Ctlcontrols { get; }

        [DispId(5)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public virtual IWMPSettings settings { get; }

        [DispId(6)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public virtual IWMPMedia currentMedia { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DispId(8)]
        public virtual IWMPMediaCollection mediaCollection { get; }

        [DispId(9)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public virtual IWMPPlaylistCollection playlistCollection { get; }

        [DispId(11)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual string versionInfo { get; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DispId(7)]
        public virtual IWMPNetwork network { get; }

        [DispId(13)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public virtual IWMPPlaylist currentPlaylist { get; set; }

        [DispId(14)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public virtual IWMPCdromCollection cdromCollection { get; }

        [DispId(15)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public virtual IWMPClosedCaption closedCaption { get; }

        [DispId(16)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool isOnline { get; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [DispId(17)]
        public virtual IWMPError Error { get; }

        [DispId(18)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual string status { get; }

        [Browsable(false)]
        [DispId(40)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual IWMPDVD dvd { get; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DispId(19)]
        public virtual bool Ctlenabled { get; set; }

        [DispId(21)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool fullScreen { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DispId(22)]
        public virtual bool enableContextMenu { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DispId(23)]
        public virtual string uiMode { get; set; }

        [DispId(24)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool stretchToFit { get; set; }

        [DispId(25)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool windowlessVideo { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DispId(26)]
        public virtual bool isRemote { get; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DispId(27)]
        public virtual IWMPPlayerApplication playerApplication { get; }

        public virtual void openPlayer(string bstrURL);
        public virtual void close();
        public virtual void launchURL(string bstrURL);
        public virtual IWMPPlaylist newPlaylist(string bstrName, string bstrURL);
        public virtual IWMPMedia newMedia(string bstrURL);
        protected override void CreateSink();
        protected override void DetachSink();
        protected override void AttachInterfaces();

        public event AxWMPLib._WMPOCXEvents_DomainChangeEventHandler DomainChange;
        public event EventHandler SwitchedToPlayerApplication;
        public event EventHandler SwitchedToControl;
        public event EventHandler PlayerDockedStateChange;
        public event EventHandler PlayerReconnect;
        public event AxWMPLib._WMPOCXEvents_ClickEventHandler ClickEvent;
        public event AxWMPLib._WMPOCXEvents_DoubleClickEventHandler DoubleClickEvent;
        public event AxWMPLib._WMPOCXEvents_KeyDownEventHandler KeyDownEvent;
        public event AxWMPLib._WMPOCXEvents_KeyPressEventHandler KeyPressEvent;
        public event AxWMPLib._WMPOCXEvents_KeyUpEventHandler KeyUpEvent;
        public event AxWMPLib._WMPOCXEvents_MouseDownEventHandler MouseDownEvent;
        public event AxWMPLib._WMPOCXEvents_MouseMoveEventHandler MouseMoveEvent;
        public event AxWMPLib._WMPOCXEvents_MouseUpEventHandler MouseUpEvent;
        public event AxWMPLib._WMPOCXEvents_DeviceConnectEventHandler DeviceConnect;
        public event AxWMPLib._WMPOCXEvents_DeviceDisconnectEventHandler DeviceDisconnect;
        public event AxWMPLib._WMPOCXEvents_DeviceStatusChangeEventHandler DeviceStatusChange;
        public event AxWMPLib._WMPOCXEvents_DeviceSyncStateChangeEventHandler DeviceSyncStateChange;
        public event AxWMPLib._WMPOCXEvents_DeviceSyncErrorEventHandler DeviceSyncError;
        public event AxWMPLib._WMPOCXEvents_CreatePartnershipCompleteEventHandler CreatePartnershipComplete;
        public event AxWMPLib._WMPOCXEvents_DeviceEstimationEventHandler DeviceEstimation;
        public event AxWMPLib._WMPOCXEvents_CdromRipStateChangeEventHandler CdromRipStateChange;
        public event AxWMPLib._WMPOCXEvents_CdromRipMediaErrorEventHandler CdromRipMediaError;
        public event AxWMPLib._WMPOCXEvents_CdromBurnStateChangeEventHandler CdromBurnStateChange;
        public event AxWMPLib._WMPOCXEvents_CdromBurnMediaErrorEventHandler CdromBurnMediaError;
        public event AxWMPLib._WMPOCXEvents_CdromBurnErrorEventHandler CdromBurnError;
        public event AxWMPLib._WMPOCXEvents_LibraryConnectEventHandler LibraryConnect;
        public event AxWMPLib._WMPOCXEvents_LibraryDisconnectEventHandler LibraryDisconnect;
        public event AxWMPLib._WMPOCXEvents_FolderScanStateChangeEventHandler FolderScanStateChange;
        public event AxWMPLib._WMPOCXEvents_StringCollectionChangeEventHandler StringCollectionChange;
        public event AxWMPLib._WMPOCXEvents_MediaCollectionMediaAddedEventHandler MediaCollectionMediaAdded;
        public event AxWMPLib._WMPOCXEvents_MediaCollectionMediaRemovedEventHandler MediaCollectionMediaRemoved;
        public event AxWMPLib._WMPOCXEvents_OpenStateChangeEventHandler OpenStateChange;
        public event AxWMPLib._WMPOCXEvents_PlayStateChangeEventHandler PlayStateChange;
        public event AxWMPLib._WMPOCXEvents_AudioLanguageChangeEventHandler AudioLanguageChange;
        public event EventHandler StatusChange;
        public event AxWMPLib._WMPOCXEvents_ScriptCommandEventHandler ScriptCommand;
        public event EventHandler NewStream;
        public event AxWMPLib._WMPOCXEvents_DisconnectEventHandler Disconnect;
        public event AxWMPLib._WMPOCXEvents_BufferingEventHandler Buffering;
        public event EventHandler ErrorEvent;
        public event AxWMPLib._WMPOCXEvents_WarningEventHandler Warning;
        public event AxWMPLib._WMPOCXEvents_EndOfStreamEventHandler EndOfStream;
        public event AxWMPLib._WMPOCXEvents_PositionChangeEventHandler PositionChange;
        public event AxWMPLib._WMPOCXEvents_MarkerHitEventHandler MarkerHit;
        public event AxWMPLib._WMPOCXEvents_DurationUnitChangeEventHandler DurationUnitChange;
        public event AxWMPLib._WMPOCXEvents_CdromMediaChangeEventHandler CdromMediaChange;
        public event AxWMPLib._WMPOCXEvents_PlaylistChangeEventHandler PlaylistChange;
        public event AxWMPLib._WMPOCXEvents_CurrentPlaylistChangeEventHandler CurrentPlaylistChange;
        public event AxWMPLib._WMPOCXEvents_CurrentPlaylistItemAvailableEventHandler CurrentPlaylistItemAvailable;
        public event AxWMPLib._WMPOCXEvents_MediaChangeEventHandler MediaChange;
        public event AxWMPLib._WMPOCXEvents_CurrentMediaItemAvailableEventHandler CurrentMediaItemAvailable;
        public event AxWMPLib._WMPOCXEvents_CurrentItemChangeEventHandler CurrentItemChange;
        public event EventHandler MediaCollectionChange;
        public event AxWMPLib._WMPOCXEvents_MediaCollectionAttributeStringAddedEventHandler MediaCollectionAttributeStringAdded;
        public event AxWMPLib._WMPOCXEvents_MediaCollectionAttributeStringRemovedEventHandler MediaCollectionAttributeStringRemoved;
        public event AxWMPLib._WMPOCXEvents_MediaCollectionAttributeStringChangedEventHandler MediaCollectionAttributeStringChanged;
        public event EventHandler PlaylistCollectionChange;
        public event AxWMPLib._WMPOCXEvents_PlaylistCollectionPlaylistAddedEventHandler PlaylistCollectionPlaylistAdded;
        public event AxWMPLib._WMPOCXEvents_PlaylistCollectionPlaylistRemovedEventHandler PlaylistCollectionPlaylistRemoved;
        public event AxWMPLib._WMPOCXEvents_PlaylistCollectionPlaylistSetAsDeletedEventHandler PlaylistCollectionPlaylistSetAsDeleted;
        public event AxWMPLib._WMPOCXEvents_ModeChangeEventHandler ModeChange;
        public event AxWMPLib._WMPOCXEvents_MediaErrorEventHandler MediaError;
        public event AxWMPLib._WMPOCXEvents_OpenPlaylistSwitchEventHandler OpenPlaylistSwitch;
    }
}
