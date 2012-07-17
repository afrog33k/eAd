using System;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc.Html;
using System.Web.WebPages;
using eAd.Website.Repositories;
using irio.utilities;

namespace eAd.Website.CustomControls
{
    public class FileUpload
    {
        public static string UploadIDTag = "::IRIO_UPLOAD_ID::";
        //rendering the dialog box
        public static void Render(string name, string action, string legend, string label, string onComplete)
        {
           
           var insideView = ((System.Web.Mvc.WebViewPage)WebPageContext.Current.Page);
           UploadRepository.CreateUploadGUID(insideView.Context);
           var uploadid = UploadIDTag + Guid.NewGuid().ToString();
            insideView.Html.RenderPartial(
                "FileUpload",
                new FileUpload()
                {
                    Name = name,
                    OnComplete = onComplete,
                    Action = action,
                    Title = legend,
                    Label = label,
                    UploadID = uploadid,
                    MaxFileSize = PrettyPrinter.FormatByteCount((ulong)((HttpRuntimeSection)ConfigurationManager.GetSection("system.web/httpRuntime")).MaxRequestLength * 100)
                });
        }

        public string OnComplete { get; set; }

        public string UploadID { get; set; }

        //Properties
        public string Name { get; set; }
        public string Action { get; set; }
        public string Title { get; set; }
        public string Label { get; set; }

        public string MaxFileSize { get; set; }
    }
}