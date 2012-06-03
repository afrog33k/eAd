namespace ClientApp
{
    using ClientApp.Core;
    using ClientApp.Properties;
    using eAd.DataViewModels;
    using eAd.Utilities;
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;

    public class CacheManager
    {
        private object CacheManagerFileLock = new object();
        public Collection<Md5Resource> Files = new Collection<Md5Resource>();

        public void Add(string path, string md5)
        {
            foreach (Md5Resource resource in this.Files)
            {
                if (resource.Path == path)
                {
                    return;
                }
            }
            Md5Resource item = new Md5Resource {
                Path = path,
                MD5 = md5,
                CacheDate = DateTime.Now
            };
            this.Files.Add(item);
        }

        private string CalcMD5(string path)
        {
            string str;
            try
            {
                using (FileStream stream = new FileStream(Settings.Default.LibraryPath + @"\" + path, FileMode.Open, FileAccess.Read))
                {
                    str = Hashes.MD5(stream);
                }
            }
            catch (Exception exception)
            {
                Trace.WriteLine(new LogMessage("CalcMD5", "Unable to calc the MD5 because: " + exception.Message), LogType.Error.ToString());
                str = "0";
            }
            return str;
        }

        public string GetMD5(string path)
        {
            foreach (Md5Resource resource in this.Files)
            {
                if (resource.Path == path)
                {
                    if (File.GetLastWriteTime(Settings.Default.LibraryPath + @"\" + path) > resource.CacheDate)
                    {
                        string str = this.CalcMD5(path);
                        this.Remove(path);
                        this.Add(path, str);
                        return str;
                    }
                    return resource.MD5;
                }
            }
            return this.CalcMD5(path);
        }

        public bool IsValidLayout(string layoutFile)
        {
            layoutFile = @"Layouts\" + layoutFile;
            if (!this.IsValidPath(layoutFile))
            {
                return false;
            }
            using (FileStream stream = new FileStream(Settings.Default.LibraryPath + @"\" + layoutFile, FileMode.Open))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(LayoutModel));
                LayoutModel model = (LayoutModel) serializer.Deserialize(stream);
                foreach (LayoutRegion region in model.Regions)
                {
                    foreach (LayoutRegionMedia media in region.Media)
                    {
                        string str;
                        if ((((str = media.Type) != null) && (((str == "Video") || (str == "Image")) || ((str == "Flash") || (str == "Ppt")))) && !this.IsValidPath(media.Options.Uri))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public bool IsValidPath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                foreach (Md5Resource resource in this.Files)
                {
                    if (resource.Path == path)
                    {
                        if (resource.CacheDate > DateTime.Now.AddMinutes(-2.0))
                        {
                            return true;
                        }
                        try
                        {
                            return (File.GetLastWriteTime(Settings.Default.LibraryPath + @"\" + path) <= resource.CacheDate);
                        }
                        catch (Exception exception)
                        {
                            Trace.WriteLine(new LogMessage("IsValid", "Unable to determine if the file is valid. Assuming not valid: " + exception.Message), LogType.Error.ToString());
                            return false;
                        }
                    }
                }
            }
            return false;
        }

        public void Regenerate()
        {
            if (File.Exists(App.UserAppDataPath + @"\" + Settings.Default.RequiredFilesFile))
            {
                XmlDocument document = new XmlDocument();
                document.Load(App.UserAppDataPath + @"\" + Settings.Default.RequiredFilesFile);
                foreach (System.Xml.XmlNode node in document.SelectNodes("//RequiredFileModel/Path"))
                {
                    string innerText = node.InnerText;
                    if (File.Exists(Settings.Default.LibraryPath + @"\" + innerText))
                    {
                        this.Add(innerText, this.GetMD5(innerText));
                    }
                }
            }
        }

        public void Remove(string path)
        {
            for (int i = 0; i < this.Files.Count; i++)
            {
                Md5Resource item = this.Files[i];
                if (item.Path == path)
                {
                    this.Files.Remove(item);
                }
            }
        }

        public void WriteCacheManager()
        {
            lock (this.CacheManagerFileLock)
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(App.UserAppDataPath + @"\" + Settings.Default.CacheManagerFile))
                    {
                        new XmlSerializer(typeof(CacheManager)).Serialize((TextWriter) writer, this);
                    }
                }
                catch (Exception exception)
                {
                    Trace.WriteLine(new LogMessage("MainForm_FormClosing", "Unable to write CacheManager to disk because: " + exception.Message));
                }
            }
        }
    }
}

