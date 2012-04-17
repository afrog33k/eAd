using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Xml.Serialization;
using Client.Properties;
using Client.Service;
using eAd.DataViewModels;

namespace Client.Core
{
    public class RequiredFiles
    {
    
        public List<RequiredFileModel> _requiredFiles;
        private ServiceClient _report;

        public List<RequiredFileModel> Files
        {
            get { return _requiredFiles; }
            set { 
                _requiredFiles = value;
                SetRequiredFiles();
            }
        }

        public RequiredFiles()
        {
            _requiredFiles = new List<RequiredFileModel>();

            // Create a webservice call
            _report = new ServiceClient();

            // Start up the Xmds Service Object
            //_report.Credentials = null;
            //_report.Url = Properties.Settings.Default.XiboClient_xmds_xmds;
            //_report.UseDefaultCredentials = false;
        }

        /// <summary>
        /// Set required files from the XML document
        /// </summary>
        private void SetRequiredFiles()
        {
            List<RequiredFileModel> newList = new List<RequiredFileModel>();
            foreach (var file in _requiredFiles)
            {
                RequiredFileModel rf = new RequiredFileModel();

               
                rf.FileType =file.FileType;
                rf.Complete = false;
                rf.MD5 = file.MD5;
                rf.LastChecked = DateTime.Now;
                rf.Size = file.Size;

                if (rf.FileType == "media")
                {
                    string[] filePart = file.Path.Split('.');
                    rf.Id = file.Id;//int.Parse(filePart[0]);
                    rf.Path =file.Path;
                }
                else if (rf.FileType == "layout")
                {
                    rf.Id = int.Parse(file.Path);
                    rf.Path = file.Path + ".mosaic";//attributes["path"].Value + ".mosaic";
                }
                else
                {
                    continue;
                }

                newList.Add(rf);
            }
            _requiredFiles = newList;
        }


        /// <summary>
        /// Mark a RequiredFileModel as complete
        /// </summary>
        /// <param name="path"></param>
        /// <param name="md5"></param>
        public void MarkComplete(string path, string md5)
        {
            foreach (RequiredFileModel rf in _requiredFiles)
            {
                if (rf.Path == path)
                {
                    RequiredFileModel newRf = rf;

                    newRf.Complete = true;
                    newRf.MD5 = md5;


                    _requiredFiles.Add(newRf);
                    _requiredFiles.Remove(rf);

                    return;
                }
            }
        }

        /// <summary>
        /// Mark a RequiredFileModel as incomplete
        /// </summary>
        /// <param name="id"></param>
        /// <param name="md5"></param>
        public void MarkIncomplete(int id, string md5)
        {
            foreach (RequiredFileModel rf in _requiredFiles)
            {
                if (rf.Id == id)
                {
                    RequiredFileModel newRf = rf;

                    newRf.Complete = false;
                    newRf.MD5 = md5;

                    _requiredFiles.Add(newRf);
                    _requiredFiles.Remove(rf);

                    return;
                }
            }
        }

        /// <summary>
        /// Writes Required Files to disk
        /// </summary>
        public void WriteRequiredFiles()
        {
            Debug.WriteLine(new LogMessage("RequiredFiles - WriteRequiredFiles", "About to Write RequiredFiles"), LogType.Info.ToString());

            try
            {
                using (StreamWriter streamWriter = new StreamWriter(App.UserAppDataPath + "\\" + Settings.Default.RequiredFilesFile))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(RequiredFiles));

                    xmlSerializer.Serialize(streamWriter, this);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(new LogMessage("RequiredFiles - WriteRequiredFiles", "Unable to write RequiredFiles to disk because: " + ex.Message));
            }
        }

        /// <summary>
        /// Report Required Files to XMDS
        /// </summary>
        public void ReportInventory()
        {
            HardwareKey hardwareKey = new HardwareKey();

            // Build the XML required by media file
            string xml = "";
            
            foreach (RequiredFileModel rf in _requiredFiles)
            {
                xml += string.Format("<file type=\"{0}\" id=\"{1}\" complete=\"{2}\" lastChecked=\"{3}\" md5=\"{4}\" />", 
                    rf.FileType, rf.Id.ToString(), rf.Complete.ToString(), rf.LastChecked.ToString(), rf.MD5);
            }

            xml = string.Format("<files macAddress=\"{1}\">{0}</files>", xml, hardwareKey.MacAddress);

            _report.MediaInventoryAsync(Settings.Default.Version, Settings.Default.ServerKey,
                hardwareKey.Key, xml);
        }
    }

    
}
