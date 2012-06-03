using ClientApp.Service;

namespace ClientApp.Core
{
    using ClientApp;
    using ClientApp.Properties;
    
    using eAd.DataViewModels;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Xml.Serialization;

    public class RequiredFiles
    {
        private ServiceClient _report = new ServiceClient();
        public List<RequiredFileModel> _requiredFiles = new List<RequiredFileModel>();
        private static object RequiredFilesLock = new object();

        public void MarkComplete(string path, string md5)
        {
            foreach (RequiredFileModel model in this._requiredFiles)
            {
                if (model.Path == path)
                {
                    RequiredFileModel item = model;
                    item.Complete = true;
                    item.MD5 = md5;
                    this._requiredFiles.Add(item);
                    this._requiredFiles.Remove(model);
                    break;
                }
            }
        }

        public void MarkIncomplete(int id, string md5)
        {
            foreach (RequiredFileModel model in this._requiredFiles)
            {
                if (model.Id == id)
                {
                    RequiredFileModel item = model;
                    item.Complete = false;
                    item.MD5 = md5;
                    this._requiredFiles.Add(item);
                    this._requiredFiles.Remove(model);
                    break;
                }
            }
        }

        public void ReportInventory()
        {
            HardwareKey key = new HardwareKey();
            string str = "";
            foreach (RequiredFileModel model in this._requiredFiles)
            {
                str = str + string.Format("<file type=\"{0}\" id=\"{1}\" complete=\"{2}\" lastChecked=\"{3}\" md5=\"{4}\" />", new object[] { model.FileType, model.Id.ToString(), model.Complete.ToString(), model.LastChecked.ToString(), model.MD5 });
            }
            str = string.Format("<files macAddress=\"{1}\">{0}</files>", str, key.MacAddress);
            this._report.MediaInventoryAsync(Settings.Default.Version, Settings.Default.ServerKey, key.Key, str);
        }

        private void SetRequiredFiles()
        {
            List<RequiredFileModel> list = new List<RequiredFileModel>();
            foreach (RequiredFileModel model in this._requiredFiles)
            {
                RequiredFileModel item = new RequiredFileModel {
                    FileType = model.FileType,
                    Complete = false,
                    MD5 = model.MD5,
                    LastChecked = DateTime.Now,
                    Size = model.Size
                };
                if (item.FileType == "CurrentMedia")
                {
                    model.Path.Split(new char[] { '.' });
                    item.Id = model.Id;
                    item.Path = model.Path;
                }
                else
                {
                    if (!(item.FileType == "layout"))
                    {
                        continue;
                    }
                    item.Id = int.Parse(model.Path);
                    item.Path = model.Path + ".mosaic";
                }
                list.Add(item);
            }
            this._requiredFiles = list;
        }

        public void WriteRequiredFiles()
        {
            lock (RequiredFilesLock)
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(App.UserAppDataPath + @"\" + Settings.Default.RequiredFilesFile))
                    {
                        new XmlSerializer(typeof(RequiredFiles)).Serialize((TextWriter) writer, this);
                    }
                }
                catch (Exception exception)
                {
                    Trace.WriteLine(new LogMessage("RequiredFiles - WriteRequiredFiles", "Unable to write RequiredFiles to disk because: " + exception.Message));
                }
            }
        }

        public List<RequiredFileModel> Files
        {
            get
            {
                return this._requiredFiles;
            }
            set
            {
                this._requiredFiles = value;
                this.SetRequiredFiles();
            }
        }
    }
}

