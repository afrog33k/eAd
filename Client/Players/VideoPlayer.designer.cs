using System.Threading;
using System.Windows.Threading;

namespace Client
{
    sealed partial class VideoPlayer
    {
     
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        public override void Dispose()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(() =>
            {
              
       if (axWindowsMediaPlayer1!=null)
            {
                axWindowsMediaPlayer1.close();
          
                axWindowsMediaPlayer1.Dispose();
                axWindowsMediaPlayer1 = null;
            }
            if ( (components != null))
            {
                components.Dispose();
            }
            }));
            base.Dispose();
        }

       

        private AxWMPLib.AxWindowsMediaPlayer axWindowsMediaPlayer1;
    }
}