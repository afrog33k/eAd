using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eAd.Utilities;
using eAd.Website.Extensions;
using eAd.Website.Repositories;
using irio.mvc.fileupload;

namespace eAd.Website.Controllers
{

public class UploadController : Controller
{
    private UploadType _type = UploadType.Media;
    private string _currentMedia = "";
    string _path
    {
        get
        {
            return HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Authority + HttpContext.Request.ApplicationPath;
        }
    }



    // [Authorize]
    public ActionResult Index(string MediaId, UploadType type)
    {
        switch (type)
        {
        case UploadType.Media:
            ViewBag.Name = "MediaUploadForm";
            break;

        default:
            ViewBag.Name = "MediaUploadForm";
            break;
        }
        ViewBag.autoUpload = true;
        ViewBag.multiple = false;
        ViewBag.Type = _type;
        _type = type;

        if (HttpContext.Session != null)
            _currentMedia = (string)HttpContext.Session["CurrentMedia"];
        //if (HttpContext.Session != null)
        //    HttpContext.Session["CurrentMedia"] = MediaId;

        return View();
    }


    /// <summary>
    /// Upload a file and return a JSON result
    /// </summary>
    /// <param name="file">The file to upload.</param>
    /// <param name="fileCollection"></param>
    /// <param name="type"> type of upload</param>
    /// <returns>a FileUploadJsonResult</returns>
    /// <remarks>
    /// It is not possible to upload files using the browser's XMLHttpRequest
    /// object. So the jQuery Form Plugin uses a hidden iframe element. For a
    /// JSON response, a ContentType of application/json will cause bad browser
    /// behavior so the content-type must be text/html. Browsers can behave badly
    /// if you return JSON with ContentType of text/html. So you must surround
    /// the JSON in textarea tags. All this is handled nicely in the browser
    /// by the jQuery Form Plugin. But we need to overide the default behavior
    /// of the JsonResult class in order to achieve the desired result.
    /// </remarks>
    /// <seealso cref="http://malsup.com/jquery/form/#code-samples"/>
    //     [Authorize]
    public FileUploadJsonResult AjaxUpload(IEnumerable<HttpPostedFileBase> fileCollection, UploadType type, string uploadPath)
    {


        _type = type;
        // TODO: Add your business logic here and/or save the file
        //System.Threading.Thread.Sleep(2000); // Simulate a long running upload
        // Some browsers send file names with full path. This needs to be stripped.


        //string physicalPath = PathForUpload(file.FileName, 0);


        //// The files are not actually saved in this demo
        //file.SaveAs(physicalPath);
        //lastSavedFile.Add(ResolvePath(physicalPath));
        foreach (string file in Request.Files)
        {
            var postedFile = Request.Files[file];
            string thumbPath;
            TimeSpan aduration;
            string filetype;
            UploadRepository.StoreMediaTemp(HttpContext, postedFile, _type, out thumbPath, out uploadPath, out aduration, out filetype);
            // Return JSON
            if (postedFile != null)
            {
                if (Request.Url != null)
                {
                    var result=new FileUploadJsonResult
                    {
                        Data =
                        new
                        {
                            message =
                            string.Format("Uploaded {0} successfully.", postedFile.FileName),
                            thumbnail = _path +"/"+ thumbPath.Replace(Request.ServerVariables["APPL_PHYSICAL_PATH"], String.Empty),
                            type = !String.IsNullOrEmpty(filetype)?filetype : MimeExtensionHelper.FindMime(uploadPath, true),
                            text = MimeExtensionHelper.FindMime(uploadPath, true).Contains("text") ? thumbPath : "",
                            path = uploadPath.Replace(Request.ServerVariables["APPL_PHYSICAL_PATH"], String.Empty),
                            duration = aduration.ToString()
                        }
                    };
                    return result;
                }
            }
        }



        return new FileUploadJsonResult { Data = new { message = "Upload Failed" } };


    }


    public FileUploadJsonResult AjaxUploadStatus(string ID)
    {



        return new FileUploadJsonResult { Data = UploadManager.GetStatus(ID), JsonRequestBehavior = JsonRequestBehavior.AllowGet };

    }



    //public ActionResult Save(IEnumerable<HttpPostedFileBase> attachments)
    //{
    //    List<string > lastSavedFile = new List<string>();
    //    // The Name of the Upload component is "attachments"
    //    foreach (HttpPostedFileBase file in attachments)
    //    {
    //        // Some browsers send file names with full path. This needs to be stripped.


    //        //string physicalPath = PathForUpload(file.FileName, 0);


    //        //// The files are not actually saved in this demo
    //        //file.SaveAs(physicalPath);
    //        //lastSavedFile.Add(ResolvePath(physicalPath));

    //       string physicalPath= ResizePicture(file);
    //        string thumbPath= ResizePictureThumb(file);
    //        if(physicalPath!=null)
    //       lastSavedFile.Add(UploadRepository.ResolvePath(this.HttpContext,physicalPath));
    //        lastSavedFile.Add(UploadRepository.ResolvePath(this.HttpContext, thumbPath));

    //    }

    //    if (HttpContext.Session != null)
    //        HttpContext.Session["SavedFileList"] = lastSavedFile;
    //    // Return an empty string to signify success
    //    return Content("");
    //}



    //    [Authorize]
    public ActionResult Remove(string[] fileNames)
    {
        // The parameter of the Remove action must be called "fileNames"


        foreach (string fullName in fileNames)
        {
            // string physicalPath = PathForUpload(fullName,0);
            // TODO: Verify user permissions

            //    if (System.IO.File.Exists(physicalPath))
            {
                // The files are not actually removed in this demo
                // System.IO.File.Delete(physicalPath);
            }
        }

        // Return an empty string to signify success
        return Content("");
    }
}
}

