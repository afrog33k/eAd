using System.Diagnostics;

namespace ClientApp.Update
{
    using ClientApp;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Forms;
    using System.Xml.Serialization;

    public class UpdateMe
    {
        public string Downloadsurl = "http://1.9.13.61/Client/Update/";
        public static List<string> Info = new List<string>();

        private static readonly string ProcessToEnd = Process.GetCurrentProcess().ProcessName; 
        private static readonly string PostProcess = (Application.StartupPath + @"\" + ProcessToEnd + ".exe");
        public const string UPDATE_CURRENT = "No updates available for eAd Client";
        public const string UPDATE_INFO_ERROR = "Error in retrieving eAd Client information";
        public const string UPDATE_SUCCESS = "eAd Client has been successfully updated";
        private static object UpdateLock = new object();
        public static ClientApp.Update.Update Updater = new ClientApp.Update.Update();
        public const string UPDATER_PREFIX = "M1234_";
        public static string UpdaterPath = (Application.StartupPath + @"\Update\Updater.exe");
        public string Versionfilename = "UpdateInfo.txt";

        static  UpdateMe()
        {
            ProcessToEnd = Process.GetCurrentProcess().ProcessName;
            PostProcess = (Application.StartupPath + @"\" + ProcessToEnd + ".exe");
        }

        public void CheckForUpdate()
        {
            lock (UpdateLock)
            {
                Info = ClientApp.Update.Update.GetUpdateInfo(this.Downloadsurl, this.Versionfilename, Application.StartupPath + @"\", 1);
                if (Info != null)
                {
                    string thisversion = Thisversion;
                    if (decimal.Parse(Info[1]) > decimal.Parse(thisversion))
                    {
                        this.StartUpdate();
                    }
                }
            }
        }

        public void StartUpdate()
        {
            lock (UpdateLock)
            {
                ClientApp.Update.Update.InstallUpdateRestart(Info[3], Info[4], "\"" + Application.StartupPath + @"\", ProcessToEnd, PostProcess, "updated", UpdaterPath);
                App.Close();
            }
        }

        private void UnpackCommandline()
        {
            bool flag = false;
            string str = "";
            foreach (string str2 in Environment.GetCommandLineArgs())
            {
                if (!flag)
                {
                    flag = str2.Trim().StartsWith("/");
                }
                if (flag)
                {
                    str = str + str2;
                }
            }
            if (flag)
            {
                bool flag1 = str.Remove(0, 2) == "updated";
            }
        }

        public void UpdateMeLoad(object sender, EventArgs e)
        {
            ClientApp.Update.Update.UpdateMe("M1234_", Application.StartupPath + @"\");
            this.UnpackCommandline();
        }

        public static string Thisversion
        {
            get
            {
                string str;
                try
                {
                    using (FileStream stream = new FileStream("version.xml", FileMode.Open, FileAccess.ReadWrite))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(VersionInfo));
                        str = ((VersionInfo) serializer.Deserialize(stream)).ToString();
                    }
                }
                catch (Exception)
                {
                    File.Delete("version.xml");
                    VersionInfo o = new VersionInfo(1, 0);
                    try
                    {
                        using (FileStream stream2 = new FileStream("version.xml", FileMode.CreateNew, FileAccess.ReadWrite))
                        {
                            XmlSerializer serializer2 = new XmlSerializer(typeof(VersionInfo));
                            serializer2.Serialize((Stream) stream2, o);
                            str = (string) serializer2.Deserialize(stream2);
                        }
                    }
                    catch (Exception)
                    {
                        str = "1.00";
                    }
                }
                return str;
            }
            set
            {
                using (FileStream stream = new FileStream("version.xml", FileMode.CreateNew, FileAccess.ReadWrite))
                {
                    new XmlSerializer(typeof(VersionInfo)).Serialize((Stream) stream, value);
                }
            }
        }
    }
}

