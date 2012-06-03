﻿using System;
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


                try
                {


                    using (var stream = new FileStream((System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + ServerPath + filePath), FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))//return actual byte stream
                    {

                        stream.Seek(chunkOffset, SeekOrigin.Begin);



                        Byte[] documentcontents = new Byte[chuckSize];

                        int actual = stream.Read(documentcontents, 0, (int)chuckSize);



                        return documentcontents.Take(actual).ToArray();

                    }
                }
                catch (Exception)
                {

                    return new byte[0];
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

        static object RequiredFilesLock = new object();

        public FilesModel RequiredFiles(string serverKey, string hardwareKey, string version)
        {
            lock (RequiredFilesLock)
            {


                // Get Mosaic For station
                var container = new eAdDataContainer();

                var thisStation = (from s in container.Stations
                                   where s.HardwareKey == hardwareKey
                                   select s).FirstOrDefault<Station>();

                if (thisStation == null)
                {
                    thisStation = new Station();
                    thisStation.HardwareKey = hardwareKey;
                    thisStation.Name = hardwareKey;
                    thisStation.LastCheckIn = DateTime.Now;
                    thisStation.Status = "Registered";
                    container.Stations.AddObject(thisStation);
                    container.SaveChanges();
                }
                else
                {
                    thisStation.LastCheckIn = DateTime.Now;
                    thisStation.Status = "Online";
                    container.SaveChanges();
                }

                Mosaic mosaic = null;
                Mosaic profilemosaic = null;
                if (thisStation != null)
                {
                    var grouping = (from m in container.Groupings
                                    where m.Mosaic != null
                                    select m).FirstOrDefault<Grouping>();

                    if (grouping != null)
                    {
                        mosaic = grouping.Mosaic;
                        profilemosaic = container.Mosaics.Where(mo => mo.MosaicID == grouping.ProfileMosaicID).FirstOrDefault();
                    }
                }

                if (mosaic == null)
                {
                    mosaic = (from m in container.Mosaics
                              where m.Name.Contains("as")
                              select m).FirstOrDefault();
                }

                if (profilemosaic == null)
                {
                    profilemosaic = container.Mosaics.Where(m => m.Type == "Profile").FirstOrDefault();
                }

                var mosaicCache = AppPath + "Layouts\\" + mosaic.MosaicID + ".mosaic";
                var profilemosaicCache = AppPath + "Layouts\\" + profilemosaic.MosaicID + ".mosaic";

                if (File.Exists(mosaicCache))
                {
                    if (new FileInfo(mosaicCache).CreationTime < mosaic.Updated)
                    {
                        UpdateMosaicCache(mosaic, mosaicCache);

                    }


                }
                else
                {
                    UpdateMosaicCache(mosaic, mosaicCache);

                }

                if (File.Exists(profilemosaicCache))
                {
                    if (new FileInfo(profilemosaicCache).CreationTime < mosaic.Updated)
                    {
                        UpdateMosaicCache(profilemosaic, profilemosaicCache);

                    }


                }
                else
                {
                    UpdateMosaicCache(profilemosaic, profilemosaicCache);

                }


                container.SaveChanges();
                var files = CreateFileModel(new List<Mosaic> { mosaic, profilemosaic });
                return files;
            }
        }

        private static FilesModel CreateFileModel(List<Mosaic> mosaics)
        {


            FilesModel files = new FilesModel();

            var list = new List<RequiredFileModel>();

            // Get Mosaic For station
            var container = new eAdDataContainer();



            foreach (var mosaic in mosaics)
            {

                var mosaicCache = AppPath + "Layouts\\" + mosaic.MosaicID + ".mosaic";
                var allMedia = mosaic.Positions.SelectMany(p => p.Media);

                foreach (var medium in allMedia)
                {
                    if (list.Where(l => l.Id == medium.MediaID).Count() <= 0)
                    {
                        bool shouldCalculatenewHash = false;

                        if (medium.Hash == null || medium.Size == 0)
                        {
                            shouldCalculatenewHash = true;
                        }

                        // Calculate new hash/size
                        if (shouldCalculatenewHash)
                        {
                            try
                            {


                                using (
                                    var fs =
                                        new FileStream(
                                            System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + ServerPath +
                                            medium.Location, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                                {
                                    medium.Hash = Hashes.MD5(fs);
                                    medium.Size =
                                        new FileInfo(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath +
                                                     ServerPath + medium.Location).Length;
                                }
                            }
                            catch (Exception)
                            {


                            }
                        }

                        list.Add(new RequiredFileModel()
                                     {
                                         FileType = "media",
                                         Path = medium.Location,
                                         Id = medium.MediaID,
                                         Size = (long)medium.Size,
                                         MD5 = medium.Hash
                                     });
                    }
                }

                if (!String.IsNullOrEmpty(mosaic.Background))
                    if (list.Where(l => l.Path == mosaic.Background).Count() <= 0)
                    // Add Background to list if not already there
                    {
                        var allmedia = (from s in container.Media
                                        where s.Location == mosaic.Background
                                        select s).FirstOrDefault();

                        if (allmedia != null)
                            list.Add(new RequiredFileModel
                                         {
                                             FileType = "media",
                                             Path = mosaic.Background,
                                             Id = mosaic.MosaicID,
                                             Size = allmedia.Size,
                                             //    (long)
                                             //    new FileInfo(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath +
                                             //               ServerPath + mosaic.Background).Length,
                                             MD5 = allmedia.Hash
                                             //    Hashes.MD5(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath +
                                             //    ServerPath + mosaic.Background)
                                         });
                    }

                list.Add(new RequiredFileModel
                             {
                                 FileType = "layout",
                                 Path = "Layouts\\" + Path.GetFileName(mosaicCache),
                                 Id = mosaic.MosaicID,
                                 Size = (long)mosaic.Size,
                                 MD5 = mosaic.Hash
                             });
            }

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
                model.Height = (int)mosaic.Height;
                model.Width = (int)mosaic.Width;
                model.SchemaVersion = 1;
                model.Tags = new List<LayoutTags>();
                model.Type = mosaic.Type;
                model.Regions = new List<LayoutRegion>();

                if (model.Type == "Profile")
                {
                    //Load Data
                    XmlSerializer xserializer = new XmlSerializer(typeof(List<PositionViewModel>));
                    var extraItems =
                        (xserializer.
                        Deserialize(new StringReader(mosaic.ExtraData))
                        as List<PositionViewModel>).Select(position => new LayoutRegion
                    {
                        Height = (int)position.Height,
                        Width = (int)position.Width,
                        Left = (int)position.X,
                        Top = (int)position.Y,
                        Type = "Widget",
                        Name = position.Name,
                        Media = new List<LayoutRegionMedia>(){new LayoutRegionMedia()
                    {
                        Duration = 1000000,
                        Id = (-1).ToString(),
                        Lkid = -1,
                        Options = new LayoutRegionMediaOptions()
                        {
                            Uri = "None"
                        },
                        Raw = new LayoutRegionMediaRaw()
                        {
                           
                        },

                        SchemaVersion = 1,
                        Type = "PlaceHolder",
                        UserId =0
                    }}
                    });

                    model.Regions.AddRange(extraItems);

                }

                foreach (var position in mosaic.Positions)
                {



                    model.Regions.Add(new LayoutRegion
                    {
                        Height = (int)position.Height,
                        Width = (int)position.Width,
                        Id = position.PositionID.ToString(),
                        Left = (int)position.X,
                        Top = (int)position.Y,
                        UserId = (int)position.UserID,
                        Media = position.Media.Select(medium => new LayoutRegionMedia()
                        {
                            Duration = (int) medium.Duration.Value.TotalSeconds,
                            Id = medium.MediaID.ToString(),
                            Lkid = (int)medium.MediaID,
                            Options = new LayoutRegionMediaOptions()
                            {
                                Uri = medium.Location
                            },
                            Raw = new LayoutRegionMediaRaw()
                            {
                            },

                            SchemaVersion = 1,
                            Type = medium.Type=="Powerpoint"?"Video":medium.Type,
                            UserId = (int)medium.UserID
                        }).ToList()
                    });
                }


                XmlSerializer serializer = new XmlSerializer(typeof(LayoutModel));
                serializer.Serialize(filestream, model);
                filestream.Position = 0;
                mosaic.Hash = Hashes.MD5(filestream);
            }

            mosaic.Size = new FileInfo(mosaicCache).Length;


        }

        public ScheduleModel Schedule(string serverKey, string hardwareKey, string version)
        {
            var container = new eAdDataContainer();
            Mosaic mosaic = null;
            Mosaic profilemosaic = null;
            if ((from s in container.Stations
                 where s.HardwareKey == hardwareKey
                 select s).FirstOrDefault<Station>() != null)
            {
                var grouping = (from m in container.Groupings
                                where m.Mosaic != null
                                select m).FirstOrDefault<Grouping>();

                if (grouping != null)
                {
                    mosaic = grouping.Mosaic;
                    profilemosaic = container.Mosaics.Where(mo => mo.MosaicID == grouping.ProfileMosaicID).FirstOrDefault();
                }
            }

            if (mosaic == null)
            {
                mosaic = container.Mosaics.Where(m => m.Type == "General").FirstOrDefault();
            }

            if (profilemosaic == null)
            {
                profilemosaic = container.Mosaics.Where(m => m.Type == "Profile").FirstOrDefault();
            }
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
                    Default = true,
                    Type =mosaic.Type,
                    Hash = mosaic.Hash
                },
                 new ScheduleLayout()
                {
                    File = profilemosaic.MosaicID.ToString(),
                    FromDate = "2000-01-01 00:00:00",
                    ToDate = "2030-01-19 00:00:00",
                    ScheduleId = 0,
                    Priority = 0,
                    Default = true,
                    Type =profilemosaic.Type,
                    Hash = profilemosaic.Hash
                }
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