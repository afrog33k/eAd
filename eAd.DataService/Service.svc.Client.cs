using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using eAd.DataViewModels;
using eAd.Utilities;
using irio.utilities;

namespace eAd.DataAccess
{
    public partial class Service
    {
        public void BlackList(string serverKey, string hardwareKey, int mediaId, string type, string reason, string version)
        {



        }

        public byte[] GetFile(string serverKey, string hardwareKey, string filePath, string fileType, long chunkOffset, long chuckSize, string version)
        {



            if (fileType == "layout")
            {
                ////filePath == "4.lxf" && 
                //var slayout = new LayoutModel();
                //slayout.SchemaVersion = 1;
                //slayout.Width = 800;
                //slayout.Height = 500;
                //slayout.Bgcolor = "#000000";
                //slayout.Background = "15.jpg";
                //slayout.Regions = new List<LayoutRegion>();

                //var layoutRegion = new LayoutRegion()
                //{
                //    Id = "4f877b224c5de",
                //    UserId = 1,
                //    Width = 434,
                //    Height = 340,
                //    Top = 0,
                //    Left = 0

                //};

                //layoutRegion.Media = new List<LayoutRegionMedia>();
                //layoutRegion.Media.Add(new LayoutRegionMedia()
                //{
                //    Id = 14.ToString(),
                //    Type = "image",
                //    Duration = 10,
                //    UserId = 1,
                //    SchemaVersion = 1,
                //    Lkid = 17,
                //    Options = new LayoutRegionMediaOptions() { Uri = "14.jpg" }
                //}
                //    );

                //layoutRegion.Media.Add(new LayoutRegionMedia()
                //{
                //    Id = 15.ToString(),
                //    Type = "image",
                //    Duration = 10,
                //    UserId = 1,
                //    SchemaVersion = 1,
                //    Lkid = 18,
                //    Options = new LayoutRegionMediaOptions() { Uri = "15.jpg" }
                //});


                //layoutRegion.Media.Add(new LayoutRegionMedia()
                //{
                //    Id = 12.ToString(),
                //    Type = "image",
                //    Duration = 10,
                //    UserId = 1,
                //    SchemaVersion = 1,
                //    Lkid = 19,
                //    Options = new LayoutRegionMediaOptions() { Uri = "12.jpg" }
                //});

                //layoutRegion.Media.Add(new LayoutRegionMedia()
                //{
                //    Id = 14.ToString(),
                //    Type = "image",
                //    Duration = 10,
                //    UserId = 1,
                //    SchemaVersion = 1,
                //    Lkid = 20,
                //    Options = new LayoutRegionMediaOptions() { Uri = "14.jpg" }
                //});

                //layoutRegion.Media.Add(new LayoutRegionMedia()
                //{
                //    Id = 10.ToString(),
                //    Type = "image",
                //    Duration = 10,
                //    UserId = 1,
                //    SchemaVersion = 1,
                //    Lkid = 21,
                //    Options = new LayoutRegionMediaOptions() { Uri = "10.png" }
                //});


                //slayout.Regions.Add(layoutRegion);

                //layoutRegion = new LayoutRegion()
                //{
                //    Id = "4f877de3cb81d",
                //    UserId = 1,
                //    Width = 800,
                //    Height = 117,
                //    Top = 383,
                //    Left = 0

                //};

                //layoutRegion.Media = new List<LayoutRegionMedia>();

                //layoutRegion.Media.Add(new LayoutRegionMedia()
                //{
                //    Id = "fc9b78e0049e5b6662c15753201d634c",
                //    Type = "text",
                //    Duration = 15,
                //    UserId = 1,
                //    SchemaVersion = 1,
                //    Options = new LayoutRegionMediaOptions()
                //    {
                //        Direction = "right",
                //        ScrollSpeed = 30
                //    },
                //    Raw = new LayoutRegionMediaRaw
                //    {
                //        Text = "<p><strong><span style=\"font-family:comic sans ms,cursive;\"><span style=\"font-size:48px;\">This is a Cool Scrolling Message that keeps scrolling till the end</span></span></strong></p>"
                //    }
                //}
                //    );

                //slayout.Regions.Add(layoutRegion);

                //layoutRegion = new LayoutRegion()
                //{
                //    Id = "4f877ea5e4e75",
                //    UserId = 1,
                //    Width = 325,
                //    Height = 268,
                //    Top = 81,
                //    Left = 372

                //};
                //layoutRegion.Media = new List<LayoutRegionMedia>();
                //layoutRegion.Media.Add(new LayoutRegionMedia()
                //{
                //    Id = 16.ToString(),
                //    Type = "video",
                //    Duration = 100,
                //    UserId = 1,
                //    SchemaVersion = 1,
                //    Lkid = 23,
                //    Options = new LayoutRegionMediaOptions() { Uri = "16.avi" }
                //});


                //layoutRegion.Media.Add(new LayoutRegionMedia()
                //{
                //    Id = 9.ToString(),
                //    Type = "video",
                //    Duration = 5,
                //    UserId = 1,
                //    SchemaVersion = 1,
                //    Lkid = 24,
                //    Options = new LayoutRegionMediaOptions() { Uri = "9.avi" }
                //});

                //layoutRegion.Media.Add(new LayoutRegionMedia()
                //{
                //    Id = 12.ToString(),
                //    Type = "image",
                //    Duration = 10,
                //    UserId = 1,
                //    SchemaVersion = 1,
                //    Lkid = 25,
                //    Options = new LayoutRegionMediaOptions() { Uri = "12.jpg" }
                //});

                //layoutRegion.Media.Add(new LayoutRegionMedia()
                //{
                //    Id = 16.ToString(),
                //    Type = "video",
                //    Duration = 50,
                //    UserId = 1,
                //    SchemaVersion = 1,
                //    Lkid = 21,
                //    Options = new LayoutRegionMediaOptions() { Uri = "16.avi" }
                //});


                //slayout.Regions.Add(layoutRegion);


               

                // MemoryStream stringWriter = new MemoryStream();
                //  XmlSerializer serializer = new XmlSerializer(slayout.GetType());
                //  serializer.Serialize(stringWriter, slayout);
                //   return stringWriter.ToArray();
                //     using (var stream = new StreamReader(filePath))

                //       return stream.ReadToEnd();

                var bytes = File.ReadAllBytes(AppPath + filePath);

                return bytes;
                //var text = File.ReadAllText(AppPath + filePath);
                //return text.Select(f => (byte)(f)).ToArray();
                //using (var stream = new StreamReader(HttpContext.Current.Server.MapPath("~/Media/" + filePath)))//return actual byte stream

                //{



                //    var allbytes = stream.ReadToEnd().Select(f=> (byte)(f)).ToArray();

                //    return allbytes;

                //}

            }

            else
            {



                using (var stream = new FileStream((System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + ServerPath + filePath), FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))//return actual byte stream
                {

                    stream.Seek(chunkOffset, SeekOrigin.Begin);



                    Byte[] documentcontents = new Byte[chuckSize];

                    int actual = stream.Read(documentcontents, 0, (int) chuckSize);



                    return documentcontents.Take(actual).ToArray();

                }

            }

        }

        public bool RecieveXmlLog(string serverKey, string hardwareKey, string xml, string version)
        {

            return true;

        }

        public string RegisterDisplay(string serverKey, string hardwareKey, string displayName, string version)
        {

            return "Display is active and ready to start.";

        }

        public FilesModel RequiredFiles(string serverKey, string hardwareKey, string version)
        {
            // Get Mosaic For station
            var container = new eAdDataContainer();

            var mosaic = container.Mosaics.Where(m => m.Name.Contains("Mani Mosaic")).FirstOrDefault();

            
            var mosaicCache = AppPath + "Layouts\\" + mosaic.MosaicID + ".mosaic";
            if (File.Exists(mosaicCache))
            {
                if(new FileInfo(mosaicCache).CreationTime < mosaic.Created)
                {
                    UpdateMosaicCache(mosaic,mosaicCache);
                  
                }
               
                    
            }
            else
            {
                UpdateMosaicCache(mosaic, mosaicCache);
               
            }

            container.SaveChanges();
            var files = CreateFileModel(mosaicCache, mosaic);
            return files;
         
            //   StringWriter stringWriter = new StringWriter();
            //   XmlSerializer serializer = new XmlSerializer(files.GetType());
            //   serializer.Serialize(stringWriter,files);
            ////   return stringWriter.ToString();
            //   //      using (var stream = new StreamReader(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath  +"/Media/files.xml"))

            //        return stream.ReadToEnd();



        }

        private static FilesModel CreateFileModel(string mosaicCache, Mosaic mosaic )
        {
            FilesModel files = new FilesModel();

            var list = new List<RequiredFileModel>();

            var allMedia = mosaic.Positions.SelectMany(p => p.Media);

            foreach (var medium in allMedia)
            {
                list.Add(new RequiredFileModel()
                             {
                                 FileType = "media",
                                 Path = medium.Location,
                                 Id = medium.MediaID,
                                 Size = (long) medium.Size,
                                 MD5 = medium.Hash
                             });
            }
            list.Add(new RequiredFileModel
                         {
                             FileType = "layout",
                             Path = "Layouts\\" + Path.GetFileName(mosaicCache),
                             Id = mosaic.MosaicID,
                             Size = (long) mosaic.Size,
                             MD5 = mosaic.Hash
                         });
           

            files.Items = new List<RequiredFileModel>(list.ToArray());
            return files;
        }

        private static void UpdateMosaicCache(Mosaic mosaic, string mosaicCache)
        {

            FileUtilities.FolderCreate(Path.GetDirectoryName(mosaicCache));

            File.Delete(mosaicCache);
           
            using (var filestream = new FileStream(mosaicCache, FileMode.CreateNew))
            {
                LayoutModel model = new LayoutModel();


                model.Background = mosaic.Background;
                model.Bgcolor = mosaic.BackgroundColor;
                model.Height = (int) mosaic.Height;
                model.Width = (int) mosaic.Width;
                model.SchemaVersion = 1;
                model.Tags = new List<LayoutTags>();

                model.Regions = new List<LayoutRegion>();

                foreach (var position in mosaic.Positions)
                {
                    model.Regions.Add(
                        new LayoutRegion()
                            {
                                Height = (int) position.Height,
                                Width = (int) position.Width,
                                Id = position.PositionID.ToString(),
                                Left = (int) position.X,
                                Top = (int) position.Y,
                                UserId = (int) position.UserID,
                                Media = position.Media.Select(medium => new LayoutRegionMedia()
                                                                            {
                                                                                Duration = medium.Duration.Value.Seconds,
                                                                                Id = medium.MediaID.ToString(),
                                                                                Lkid = (int) medium.MediaID,
                                                                                Options = new LayoutRegionMediaOptions()
                                                                                              {
                                                                                                  Uri = medium.Location
                                                                                              },
                                                                                Raw = new LayoutRegionMediaRaw()
                                                                                          {
                                                                                          },
                                                                                SchemaVersion = 1,
                                                                                Type = medium.Type,
                                                                                UserId = (int) medium.UserID
                                                                            }).ToList()
                            });
                }


                XmlSerializer serializer = new XmlSerializer(typeof (LayoutModel));
                serializer.Serialize(filestream, model);
                filestream.Position = 0;
                mosaic.Hash = Hashes.MD5(filestream);
            }

            mosaic.Size = new FileInfo(mosaicCache).Length;

           
        }

        public ScheduleModel Schedule(string serverKey, string hardwareKey, string version)
        {
            var container = new eAdDataContainer();
            var mosaic = container.Mosaics.Where(m => m.Name.Contains("Mani Mosaic")).FirstOrDefault();
            var schedule = new ScheduleModel()
                               {
                                   Items = new List<ScheduleLayout>()
                                               {
                                                   new ScheduleLayout()
                                                       {
                                                           File = mosaic.MosaicID.ToString(),
                                                           FromDate = "2000-01-01 00:00:00",
                                                           ToDate = "2030-01-19 00:00:00",
                                                           ScheduleId = 0,
                                                           Priority = 0,
                                                           Default = true
                                                       },
                                               }
                               };


            return schedule;

        }

        public string GetResource(string serverKey, string hardwareKey, int layoutId, string regionId, string mediaId, string version)
        {

            return "Ok";

        }

        public bool SubmitLog(string version, string serverKey, string hardwareKey, string logXml)
        {

            return true;

        }

        public bool SubmitStats(string version, string serverKey, string hardwareKey, string statXml)
        {

            return true;

        }

        public bool MediaInventory(string version, string serverKey, string hardwareKey, [System.Xml.Serialization.SoapElementAttribute("mediaInventory")] string mediaInventory1)
        {

            return true;

        }
    }
}