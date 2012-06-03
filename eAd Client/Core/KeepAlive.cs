using ClientApp.Players;

namespace ClientApp
{
    using ClientApp.Core;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;

    internal class KeepAlive : Media
    {
        private readonly string _filePath;


        public KeepAlive(RegionOptions options)
            : base(options.Width, options.Height, options.Top, options.Left)
        {
            this._filePath = options.Uri;
            
        }
    }
}

