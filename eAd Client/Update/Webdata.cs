namespace ClientApp.Update
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading;

    internal class Webdata
    {
     
        public static event BytesDownloadedEventHandler BytesDownloaded;

        public static bool DownloadFromWeb(string url, string file, string targetFolder)
        {
            try
            {
                WebResponse response = WebRequest.Create(url + file).GetResponse();
                Stream responseStream = response.GetResponseStream();
                byte[] buffer = new byte[0x400];
                int contentLength = (int) response.ContentLength;
                ByteArgs args2 = new ByteArgs {
                    Downloaded = 0,
                    Total = contentLength
                };
                ByteArgs e = args2;
                if (BytesDownloaded != null)
                {
                    BytesDownloaded(e);
                }
                MemoryStream stream2 = new MemoryStream();
                while (true)
                {
                    while (responseStream == null)
                    {
                    }
                    int count = responseStream.Read(buffer, 0, buffer.Length);
                    if (count == 0)
                    {
                        e.Downloaded = contentLength;
                        e.Total = contentLength;
                        if (BytesDownloaded != null)
                        {
                            BytesDownloaded(e);
                        }
                        break;
                    }
                    stream2.Write(buffer, 0, count);
                    e.Downloaded = count;
                    e.Total = contentLength;
                    if (BytesDownloaded != null)
                    {
                        BytesDownloaded(e);
                    }
                }
                byte[] buffer2 = stream2.ToArray();
                responseStream.Close();
                stream2.Close();
                FileStream stream3 = new FileStream(targetFolder + file, FileMode.Create);
                stream3.Write(buffer2, 0, buffer2.Length);
                stream3.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

