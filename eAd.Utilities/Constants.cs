namespace eAd.Utilities
{
using System;
using System.IO;
using System.ServiceModel;
using System.Xml.Serialization;

public class Constants
{
    public static ClientConfiguration CurrentClientConfiguration;

    static Constants()
    {
        LoadDefaults();
    }

    public static void LoadDefaults()
    {
        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ClientConfiguration));
            StreamReader textReader = File.OpenText(ClientConfiguration.ConfigurationFile);
            CurrentClientConfiguration = serializer.Deserialize(textReader) as ClientConfiguration;
            textReader.Close();
        }
        catch (Exception)
        {
            CurrentClientConfiguration = new ClientConfiguration();
            SaveDefaults();
        }
    }

    public static void SaveDefaults()
    {
        StreamWriter writer;
        XmlSerializer serializer = new XmlSerializer(typeof(ClientConfiguration));
        FileInfo info = new FileInfo(ClientConfiguration.ConfigurationFile);
        if (!File.Exists(ClientConfiguration.ConfigurationFile))
        {
            writer = info.CreateText();
            serializer.Serialize((TextWriter) writer, CurrentClientConfiguration);
            writer.Close();
        }
        else
        {
            info.Delete();
            writer = info.CreateText();
            serializer.Serialize((TextWriter) writer, CurrentClientConfiguration);
            writer.Close();
        }
    }

    public static string AppPath
    {
        get
        {
            return CurrentClientConfiguration.AppPath;
        }
    }

    public static double DefaultDuration
    {
        get
        {
            return CurrentClientConfiguration.DefaultDuration;
        }
    }

    public static int MessageWaitTime
    {
        get
        {
            return CurrentClientConfiguration.MessageWaitTime;
        }
    }

    public static long MyStationID
    {
        get
        {
            return (long) CurrentClientConfiguration.MyStationID;
        }
    }

    public static string PlayListFile
    {
        get
        {
            return CurrentClientConfiguration.PlayListFile;
        }
    }

    public static EndpointAddress ServerAddress
    {
        get
        {
            return new EndpointAddress(ServerUrl + "/DataAccess/Service.svc");
        }
    }

    public static string ServerUrl
    {
        get
        {
            return CurrentClientConfiguration.ServerUrl;
        }
    }

    public static long CurrentMosaic
    {
        get;
        set;
    }
}
}

