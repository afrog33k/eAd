using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using DesktopClient.eAdDataAccess;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using eAd.Utilities;

namespace DesktopClient
{
class Snapshot
{
    public void CaptureScreenShot()
    {
        ScreenCapture sc = new ScreenCapture();
        // capture entire screen, and save it to a file
        Image img = sc.CaptureScreen();
        // display image in a Picture control named imageDisplay
        //        this.imageDisplay.Image = img;
        // capture this window, and save it
        sc.CaptureScreenToFile("C:\\temp2.jpg", ImageFormat.Jpeg);
    }

    public static void UploadFile(string localFileName,FileTypeEnum fileType)
    {
        eAdDataAccess.ServiceClient service = new ServiceClient("BasicHttpBinding_IService", Constants.ServerAddress);
        try
        {
            using (Stream fileStream = new FileStream(localFileName, FileMode.Open, FileAccess.Read))
            {
                //    var request = new FileUploadMessage();
                string remoteFileName = null;
                if (fileType == FileTypeEnum.Generic)
                {
                    // WE ARE USING THE SERVICE AS A "FTP ON WCF"
                    // GIVE THE REMOTE FILE THE SAME NAME AS THE LOCAL ONE
                    remoteFileName = Path.GetFileName(localFileName);
                }

                var fileMetadata = new FileMetaData { LocalFilename = localFileName, RemoteFilename = remoteFileName,FileType = fileType};
                //     request.MetaData = fileMetadata;
                //     request.FileByteStream = fileStream;

                //service.UploadFile(fileMetadata,fileStream);
                service.Close();

            }
        }
        catch (TimeoutException exception)
        {
            Console.WriteLine("Got {0}", exception.GetType());
            service.Abort();
        }
        catch (CommunicationException exception)
        {
            Console.WriteLine("Got {0}", exception.GetType());
            service.Abort();
        }
    }
}
}
