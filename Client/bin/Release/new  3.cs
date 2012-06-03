/// <summary>
        /// Prepares the Layout.. rendering all the necessary controls
        /// </summary>
        private void PrepareLayout(string layoutPath)
        {
            // Create a start record for this layout
            _stat = new Stat();
            _stat.type = StatType.Layout;
            _stat.scheduleID = _scheduleId;
            _stat.layoutID = _layoutId;
            _stat.fromDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            LayoutModel layout;

            // Default or not
            if (layoutPath == Settings.Default.LibraryPath + @"\Default.xml" || String.IsNullOrEmpty(layoutPath))
            {
                throw new Exception("Default layout");
            }
            else
            {
                try
                {


                    // Get this layouts XML
                    using (var file = File.Open(layoutPath, FileMode.Open, FileAccess.Read, FileShare.Write))
                    {


                        XmlSerializer serializer = new XmlSerializer(typeof (LayoutModel));
                        layout = (LayoutModel) serializer.Deserialize(file);

                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(string.Format("Could not find the layout file {0}: {1}", layoutPath, ex.Message));
                    throw;
                }
            }

            // Attributes of the main layout node



            // Set the background and size of the form
            _layoutWidth = layout.Width;
            _layoutHeight = layout.Height;


            //// Scaling factor, will be applied to all regions
            //_scaleFactor = Math.Max(_clientSize.Width / _layoutWidth, _clientSize.Height / _layoutHeight);

            //// Want to be able to center this shiv - therefore work out which one of these is going to have left overs
            int backgroundWidth = _clientSize.Width; // (int)(_layoutWidth * _scaleFactor);
            int backgroundHeight = _clientSize.Height; //(int)(_layoutHeight * _scaleFactor);

            //double leftOverX;
            //double leftOverY;

            //try
            //{
            //    leftOverX = Math.Abs(_clientSize.Width - backgroundWidth);
            //    leftOverY = Math.Abs(_clientSize.Height - backgroundHeight);

            //    if (leftOverX != 0) leftOverX = leftOverX / 2;
            //    if (leftOverY != 0) leftOverY = leftOverY / 2;
            //}
            //catch
            //{
            //    leftOverX = 0;
            //    leftOverY = 0;
            //}


            // New region and region options objects
            _regions = new Collection<Region>();
            RegionOptions options = new RegionOptions();

            // Deal with the color
            try
            {
                if (!String.IsNullOrEmpty(layout.Bgcolor))
                {

                    //     MediaCanvas.Background =
                    //   new SolidColorBrush((Color)ColorConverter.ConvertFromString(layout.Bgcolor));
		this.BackColor= ColorTranslator.FromHtml(layout.Bgcolor);
                    options.backgroundColor = layout.Bgcolor;
                }
            }
            catch
            {

                //     MediaCanvas.Background = new SolidColorBrush(Colors.Black); // Default black



                options.backgroundColor = "#000000";
            }

            // Get the background
            try
            {
                if (layout.Background == null)
                {
                    // Assume there is no background image
                    //  MediaCanvas.Background = null;
                    options.backgroundImage = "";
                }
                else
                {

                    string bgFilePath = Settings.Default.LibraryPath + @"\backgrounds\" + backgroundWidth + "x" +
                                        backgroundHeight + "_" + layout.Background;
                    Utilities.CreateFolder(Path.GetDirectoryName(bgFilePath));
                    // Create a correctly sized background image in the temp folder
                    if (!File.Exists(bgFilePath))
                    {
                        System.Drawing.Image img =
                            System.Drawing.Image.FromFile(Settings.Default.LibraryPath + @"\" + layout.Background);

                        Bitmap bmp = new Bitmap(img, backgroundWidth, backgroundHeight);
                        EncoderParameters encoderParameters = new EncoderParameters(1);
                        EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 90L);
                        encoderParameters.Param[0] = qualityParam;

                        ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");

                        bmp.Save(bgFilePath, jpegCodec, encoderParameters);

                        img.Dispose();
                        bmp.Dispose();
                    }

			 BackgroundImage = new Bitmap(bgFilePath);

                    //   MediaCanvas.Background = new ImageBrush(new BitmapImage(new Uri(bgFilePath.Replace("\\", "/"), UriKind.Relative)));


                    options.backgroundImage = bgFilePath;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to set background: " + ex.Message);

                // Assume there is no background image
		 // Assume there is no background image
                this.BackgroundImage = null;
                //Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(() =>
                //{
                //    MediaCanvas.Background = Brushes.Black;
                //}));

                options.backgroundImage = "";
            }

            // Get it to paint the background now
            App.DoEvents();

            // Get the regions
            var listRegions = layout.Regions;
            var listMedia = layout.Regions.Select(r => r.Media).ToList();

            // Check to see if there are any regions on this layout.
            if (listRegions.Count == 0 || listMedia.Count == 0)
            {
                Trace.WriteLine(new LogMessage("PrepareLayout",
                                               string.Format(
                                                   "A layout with {0} regions and {1} media has been detected.",
                                                   listRegions.Count.ToString(), listMedia.Count.ToString())),
                                LogType.Info.ToString());

                if (_schedule.ActiveLayouts == 1)
                {
                    Trace.WriteLine(
                        new LogMessage("PrepareLayout", "Only 1 layout scheduled and it has nothing to show."),
                        LogType.Info.ToString());

                    throw new Exception("Only 1 layout schduled and it has nothing to show");
                }
                else
                {
                    Trace.WriteLine(new LogMessage("PrepareLayout",
                                                   string.Format(
                                                       string.Format(
                                                           "An empty layout detected, will show for {0} seconds.",
                                                           Settings.Default.emptyLayoutDuration.ToString()))),
                                    LogType.Info.ToString());

                    // Put a small dummy region in place, with a small dummy media node - which expires in 10 seconds.
                    // Replace the list of regions (they mean nothing as they are empty)
                    listRegions = new List<LayoutRegion>()
                                      {
                                          new LayoutRegion()
                                              {
                                                  Id = "blah",
                                                  Width = 1,
                                                  Height = 1,
                                                  Top = 1,
                                                  Left = 1,
                                                  Media = new List<LayoutRegionMedia>()
                                                              {
                                                                  new LayoutRegionMedia()
                                                                      {
                                                                          Id = "blah",
                                                                          Type = "Text",
                                                                          Duration = 0,
                                                                          Raw = new LayoutRegionMediaRaw()
                                                                                    {
                                                                                        Text = ""
                                                                                    }
                                                                      }
                                                              }

                                              }
                                      };
                }
            }

            foreach (var region in listRegions)
            {
                // Is there any media
                if (region.Media.Count == 0)
                {
                    Debug.WriteLine("A region with no media detected");
                    continue;
                }

                //each region
                options.scheduleId = _scheduleId;
                options.layoutId = _layoutId;
                options.regionId = region.Id;
                options.Width = (int) ((region.Width + 30.0)/_layoutWidth*_clientSize.Width);
                    //(int)((region.Width + 15.0) * _scaleFactor);
                options.Height = (int) ((region.Height + 30)/_layoutHeight*_clientSize.Height);
                    //(int)((region.Height + 15.0) * _scaleFactor);
                var left = region.Left - 15;
                if (left < 0)
                    left = 0;

                var top = region.Top - 15;
                if (top < 0)
                    top = 0;

                options.Left = (int) (left/_layoutWidth*_clientSize.Width); //(int)(region.Left * _scaleFactor);
                options.Top = (int) (top/_layoutHeight*_clientSize.Height); //(int)(region.Top * _scaleFactor);

                options.ScaleFactor = _scaleFactor;

                // Set the backgrounds (used for Web content offsets)
                options.BackgroundLeft = options.Left*-1;
                options.BackgroundTop = options.Top*-1;

                //Account for scaling
                //       options.Left = options.Left + (int)leftOverX;
                //        options.Top = options.Top + (int)leftOverY;

                // All the media nodes for this region / layout combination
                options.mediaNodes = region.Media;

                Region temp = new Region(ref _statLog, ref _cacheManager);
                temp.DurationElapsedEvent += new Region.DurationElapsedDelegate(TempDurationElapsedEvent);

                Debug.WriteLine("Created new region", "AdvertPlayer - Prepare Layout");

                // Dont be fooled, this innocent little statement kicks everything off
                temp.regionOptions = options;

                _regions.Add(temp);

                //    temp.Opacity = 0;

                MediaCanvas.Children.Add(temp);

                //  temp.AnimateIn();

                //          new TextBox(){
                //Text                                                                        = "Hey",
                //                                                                          Margin = new Thickness(options.left,options.top,0,0),
                //                                                                          Height = options.Height,
                //                                                                          Width = options.Width
                //                                                                        })
                ;
                //   temp.Background = new SolidColorBrush(Colors.Coral);



                Debug.WriteLine("Adding region", "AdvertPlayer - Prepare Layout");


                App.DoEvents();
            }

            // Null stuff
            listRegions = null;
            list