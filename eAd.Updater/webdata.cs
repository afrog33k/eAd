using System;
using System.IO;
using System.Net;

namespace eAd.Updater
{
internal class Webdata
{
    public static event BytesDownloadedEventHandler BytesDownloaded;

    public static bool DownloadFromWeb(string url, string file, string targetFolder)
    {
        try
        {
            byte[] downloadedData;


            downloadedData = new byte[0];

            //open a data stream from the supplied URL
            WebRequest webReq = WebRequest.Create(url + file);
            WebResponse webResponse = webReq.GetResponse();
            Stream dataStream = webResponse.GetResponseStream();

            //Download the data in chuncks
            var dataBuffer = new byte[1024];

            //Get the total size of the download
            var dataLength = (int) webResponse.ContentLength;

            //lets declare our downloaded bytes event args
            var byteArgs = new ByteArgs();

            byteArgs.Downloaded = 0;
            byteArgs.total = dataLength;

            //we need to test for a null as if an event is not consumed we will get an exception
            if (BytesDownloaded != null) BytesDownloaded(byteArgs);


            //Download the data
            var memoryStream = new MemoryStream();
            while (true)
            {
                //Let's try and read the data
                int bytesFromStream = dataStream.Read(dataBuffer, 0, dataBuffer.Length);

                if (bytesFromStream == 0)
                {
                    byteArgs.Downloaded = dataLength;
                    byteArgs.total = dataLength;
                    if (BytesDownloaded != null) BytesDownloaded(byteArgs);

                    //Download complete
                    break;
                }
                else
                {
                    //Write the downloaded data
                    memoryStream.Write(dataBuffer, 0, bytesFromStream);

                    byteArgs.Downloaded = bytesFromStream;
                    byteArgs.total = dataLength;
                    if (BytesDownloaded != null) BytesDownloaded(byteArgs);
                }
            }

            //Convert the downloaded stream to a byte array
            downloadedData = memoryStream.ToArray();

            //Release resources
            dataStream.Close();
            memoryStream.Close();

            //Write bytes to the specified file
            var newFile = new FileStream(targetFolder + file, FileMode.Create);
            newFile.Write(downloadedData, 0, downloadedData.Length);
            newFile.Close();

            return true;
        }

        catch (Exception)
        {
            //We may not be connected to the internet
            //Or the URL may be incorrect
            return false;
        }
    }
}
}