using System;
using System.IO;
using System.Xml.Serialization;

namespace eAd.Utilities
{
    public class Constants
    {
        public static ClientConfiguration
            CurrentClientConfiguration;

        public static string AppPath
        {
            get { return CurrentClientConfiguration.AppPath; }
        }

        public static double DefaultDuration
        {
            get { return CurrentClientConfiguration.DefaultDuration; }
        
        }

        public static int MessageWaitTime
        {
            get { return CurrentClientConfiguration.MessageWaitTime; }
        }

        public static long MyStationID
        {
            get { return CurrentClientConfiguration.MyStationID; }
       
        }

        public static string PlayListFile
        {
            get { return CurrentClientConfiguration.PlayListFile; }
        }

        public static string ServerUrl
        {
            get { return CurrentClientConfiguration.ServerUrl; }
        
        }

        static Constants()
        {
            LoadDefaults();
        }
        public static void   LoadDefaults()
        {
            try
            {

                XmlSerializer serializer = new XmlSerializer(typeof(ClientConfiguration));
                StreamReader reader = File.OpenText(ClientConfiguration.ConfigurationFile);
                CurrentClientConfiguration = (serializer.Deserialize(reader) as ClientConfiguration);

                reader.Close();
                
            }
            catch (Exception)
            {

                CurrentClientConfiguration = new ClientConfiguration();
                SaveDefaults();
            }

        }

        public static void SaveDefaults()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ClientConfiguration));
            FileInfo file = new FileInfo(ClientConfiguration.ConfigurationFile);
            if (!File.Exists(ClientConfiguration.ConfigurationFile))
            {
                StreamWriter writer = file.CreateText();
                serializer.Serialize(writer, CurrentClientConfiguration);
                writer.Close();
            }
            else
            {
                file.Delete();
                StreamWriter writer = file.CreateText();
                serializer.Serialize(writer, CurrentClientConfiguration);
                writer.Close();
            }
        }

      
    }

   
    }
