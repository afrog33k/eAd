using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Client
{
    public  class UpdateMe
    {
        public const string UPDATER_PREFIX = "M1234_";
        private static readonly string ProcessToEnd = System.Diagnostics.Process.GetCurrentProcess().ProcessName;

        private static readonly string PostProcess = Application.StartupPath + @"\" + ProcessToEnd + ".exe";
        public static string UpdaterPath = Application.StartupPath + @"\Updater.exe";

        public const string UPDATE_SUCCESS = "eAd Client has been successfully updated";
        public const string UPDATE_CURRENT = "No updates available for eAd Client";
        public const string UPDATE_INFO_ERROR = "Error in retrieving eAd Client information";

        public static Update Updater = new Update();

        public static List<string> Info = new List<string>();

     

        public string Thisversion {
            get
            {
                VersionInfo version = null;
                try
            {

           
            using (var serial  = new FileStream("version.xml", FileMode.Open, FileAccess.ReadWrite))
            {
                
          
            XmlSerializer serializer = new XmlSerializer(typeof(VersionInfo));
            return ((VersionInfo)serializer.Deserialize(serial)).ToString();
                 
            }
            }
            catch (Exception)
            {
                File.Delete("version.xml");
               version=  new VersionInfo(1, 0);
                try
                {
                    using (var serial = new FileStream("version.xml", FileMode.CreateNew, FileAccess.ReadWrite))
                    {


                        XmlSerializer serializer = new XmlSerializer(typeof(VersionInfo));
                        serializer.Serialize(serial, version);
                        return (string)serializer.Deserialize(serial);
                    }    
                }
                catch (Exception exception)
                {

                    return "1.00";
                }

               
            }
            
            }
            set
        {
            using (var serial = new FileStream("version.xml",FileMode.CreateNew,FileAccess.ReadWrite))
            {


                XmlSerializer serializer = new XmlSerializer(typeof(VersionInfo));
                serializer.Serialize(serial,value);

            }    
        }
        }

        public void StartUpdate()
        {

            Client.Update.InstallUpdateRestart(Info[3], Info[4], "\"" + Application.StartupPath + "\\", ProcessToEnd, PostProcess, "updated", UpdaterPath);
            App.Close();
        }

        private void UnpackCommandline()
        {

            bool commandPresent = false;
            string tempStr = "";

            foreach (string arg in Environment.GetCommandLineArgs())
            {

                if (!commandPresent)
                {

                    commandPresent = arg.Trim().StartsWith("/");

                }

                if (commandPresent)
                {

                    tempStr += arg;

                }

            }


            if (commandPresent)
            {

                if (tempStr.Remove(0, 2) == "updated")
                {

                    //  updateResult.Visible = true;
                    // updateResult.Text = UPDATE_SUCCESS;

                }

            }


        }

        public void UpdateMeLoad(object sender, EventArgs e)
        {
            Client.Update.UpdateMe(UPDATER_PREFIX, Application.StartupPath + @"\");
            //   updateResult.Visible = false;
            //  Update_bttn.Visible = false;
            UnpackCommandline();

        }

    static    object UpdateLock = new object();

        public void CheckForUpdate()
        {
            lock (UpdateLock)
            {
                Info = Client.Update.GetUpdateInfo(Downloadsurl, Versionfilename, Application.StartupPath + @"\", 1);

                if (Info == null)
                {

                    //   Update_bttn.Visible = false;
                    //    updateResult.Text = UPDATE_INFO_ERROR;
                    //    updateResult.Visible = true;
                    //TODO: Log this

                }
                else
                {

                    if (decimal.Parse(Info[1]) > decimal.Parse(Thisversion))
                    {

                        //  Update_bttn.Visible = true;
                        //    updateResult.Visible = false;

                        //Update Available ... Start update
                       StartUpdate();

                    }
                    else
                    {

                        //     Update_bttn.Visible = false;
                        //   updateResult.Visible = true;
                        //   updateResult.Text = UPDATE_CURRENT;

                    }



                }
            }
        }

        public string Versionfilename = "UpdateInfo.txt";

        public string Downloadsurl = "http://1.9.13.61/Client/Update/";
    }
}