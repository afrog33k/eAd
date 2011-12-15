using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using eAd.Utilities;
using eAd.Website.Controllers;
using irio.utilities;
using System.Linq;

namespace eAd.Website.Repositories
{
    public class UploadRepository
    {

        public static string ResolvePath(HttpContextBase context,string AbsolutePath)
        {
            string FullPath = AbsolutePath.ToLower();
            string SitePath = context.Server.MapPath("~").ToLower();
            return FullPath.Replace(SitePath, "\\");
        }

        public static string PathForUpload(HttpContextBase context,string path, string owner, string id, string filename,UploadType type, bool? isThumb = false) // 0 Media, 1 profile pic, 2 coupon
        {
            string fileName = Path.GetFileName(path);
            string physicalPath = null;
            if (fileName != null)
            {
                if (type == UploadType.Media)
                    physicalPath =
                       Path.Combine(context.Server.MapPath("~/Uploads/Media/" + owner + "/" + id),
                        ((bool)isThumb ? "Thumb" : "") + filename + Path.GetExtension(fileName));
                //else if (type == UploadType.Coupon)
                //{
                //    physicalPath =
                //   Path.Combine(context.Server.MapPath("~/Uploads/Coupons/" + owner + "/" + id),
                //            ((bool)isThumb ? "Thumb" : "") + filename + Path.GetExtension(fileName));
                //}
                //else if (type == UploadType.Logo)
                //{
                //    physicalPath =
                //   Path.Combine(context.Server.MapPath("~/Uploads/Logos/" + owner + "/" + id),
                //            ((bool)isThumb ? "Thumb" : "") + filename + Path.GetExtension(fileName));
                //}
                else if (type == UploadType.Profile)
                {
                    physicalPath =
                  Path.Combine(context.Server.MapPath("~/Uploads/Profiles/" + owner + "/" + id),
                           ((bool)isThumb ? "Thumb" : "") + filename + Path.GetExtension(fileName));
                }

            }
            return physicalPath;
        }

        public static void CreateUploadGUID(HttpContextBase context)
        {
            if(context.Session!=null)
                context.Session["UploadGUID"] = UrlFriendlyGuid.NewGuid();
        }

        public static void DeleteUploadGUID(HttpContextBase context)
        {
            if (context.Session != null)
            {

                List<UploadedContent> uploadedContents =
                    ((List<UploadedContent>) context.Session["SavedFileList"]);
              

                if (uploadedContents != null)
                {
                    UrlFriendlyGuid GUID = (UrlFriendlyGuid)context.Session["UploadGUID"];
                UploadedContent thisContent = uploadedContents.Find(
                    content =>
                    content.MediaGuid == GUID);
                uploadedContents.Remove(thisContent);

                context.Session["SavedFileList"] = uploadedContents;
                context.Session["UploadGUID"] = null;
            }
        }
        }

        public static string GetFileUrl(HttpContextBase context, string MediaID, string ownerID , UploadType type, out UploadedContent upload)
        {
            upload = null;
            try
            {

           
            if (context.Session != null)
            {
                List<UploadedContent> uploadedContents =
                    ((List<UploadedContent>) context.Session["SavedFileList"]);
                UrlFriendlyGuid GUID = (UrlFriendlyGuid)context.Session["UploadGUID"];

                UploadedContent thisContent = uploadedContents.Find(
                    content =>
                    content.MediaGuid == GUID && content.Type == type);

                upload = thisContent;

                List<string> pictures = thisContent.
                    Pictures;

              
                            string newPath = PathForUpload(context,

                                                               pictures[0],
                                                               ownerID,
                                                               MediaID,
                                                               GUID
                                                               , type,
                                                               false);

                            string thumbPath = PathForUpload(context,

                                                                               pictures[0],
                                                                              ownerID,
                                                                              MediaID,
                                                                              GUID
                                                                              , type,
                                                                              true);
              

                FileUtilities.FolderCreate(Path.GetDirectoryName(newPath));
                System.IO.File.Move(
                     TempPathForUpload(context, pictures[0], type, GUID)
                   ,
                                    newPath);
                try //Maybe No Thumbnail
                {

             
                System.IO.File.Move(
                      TempPathForUpload(context, pictures[1], type, GUID),
                                    thumbPath);
                }
                catch (Exception ex)
                {

                  Console.WriteLine("No Thumbnail" + ex.Message);
                }
                uploadedContents.Remove(thisContent);

                context.Session["SavedFileList"] =uploadedContents;

                return ResolvePath(context, newPath);
            }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                
            }
            return null;
        }

        public static string TempPathForUpload(HttpContextBase context,string path, UploadType type, string _currentMedia, bool isThumb = false)
        {
            try
            {
                string fileName = Path.GetFileName(path);
                string physicalPath = null;
                if (fileName != null)
                {
                    if (type == UploadType.Media)
                    {


                        physicalPath =
                            Path.Combine(
                                context.Server.MapPath("~/Uploads/Temp/Media/"),
                                ((bool)isThumb ? "Thumb" : "") + _currentMedia + Path.GetExtension(fileName));
                    }
                        //else if (type == UploadType.Logo)
                    //{
                    //    physicalPath =
                    //        Path.Combine(
                    //            context.Server.MapPath("~/Uploads/Temp/Logos/" + AccountsRepository.MyAccountID),
                    //            ((bool) isThumb ? "Thumb" : "") + _currentMedia + Path.GetExtension(fileName));
                    //}
                    else if (type == UploadType.Profile)
                    {
                        //physicalPath =
                        //    Path.Combine(
                        //        context.Server.MapPath("~/Uploads/Temp/Profiles/" + AccountsRepository.MyAccountID),
                        //        ((bool) isThumb ? "Thumb" : "") + _currentMedia + Path.GetExtension(fileName));

                        physicalPath =
                          Path.Combine(
                              context.Server.MapPath("~/Uploads/Temp/Profiles/"),
                              ((bool)isThumb ? "Thumb" : "") + _currentMedia + Path.GetExtension(fileName));
                    }

                    //else if (type == UploadType.Coupon)
                    //{
                    //    physicalPath =
                    //        Path.Combine(
                    //            context.Server.MapPath("~/Uploads/Temp/Coupons/" + AccountsRepository.MyAccountID),
                    //            ((bool) isThumb ? "Thumb" : "") + _currentMedia + Path.GetExtension(fileName));
                    //}

                }
                return physicalPath;
            }
            catch
            {
                
            }
            return "";
        }

        public Bitmap ProportionallyResizeBitmap(Bitmap src, int maxWidth, int maxHeight)
        {

            // original dimensions
            int w = src.Width;
            int h = src.Height;

            // Longest and shortest dimension
            int longestDimension = (w > h) ? w : h;
            int shortestDimension = (w < h) ? w : h;

            // propotionality
            float factor = ((float)longestDimension) / shortestDimension;

            // default width is greater than height
            double newWidth = maxWidth;
            double newHeight = maxWidth / factor;

            // if height greater than width recalculate
            if (w < h)
            {
                newWidth = maxHeight / factor;
                newHeight = maxHeight;
            }

            // Create new Bitmap at new dimensions
            Bitmap result = new Bitmap((int)newWidth, (int)newHeight);
            using (Graphics g = Graphics.FromImage((System.Drawing.Image)result))
                g.DrawImage(src, 0, 0, (int)newWidth, (int)newHeight);

            return result;
        }

        public  static string[] ImageExtensions = new string[]
                                                      {
                                                          ".jpg",
                                                          ".png",
                                                          ".bmp",
                                                          ".tga",
                                                          ".tif",
                                                          ".gif"
                                                      };

        public static string StoreMediaTemp(HttpContextBase context, HttpPostedFileBase file, UploadType type, out string thumbPath, out string physicalPath, out TimeSpan duration)
        {
            duration = TimeSpan.Zero;
            if (context.Session != null)
            {
                UrlFriendlyGuid GUID = (UrlFriendlyGuid) context.Session["UploadGUID"];

                List<UploadedContent> lastSavedFile = new List<UploadedContent>();

                if (context.Session["SavedFileList"] != null)
                    lastSavedFile = (List<UploadedContent>) context.Session["SavedFileList"];

                physicalPath = TempPathForUpload(context, file.FileName, type, GUID);
                thumbPath = TempPathForUpload(context, file.FileName, type, GUID, true);

                //Todo: Use mimes here instead of basic extension check
                if (ImageExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
                {
                    Bitmap image = (Bitmap) Bitmap.FromStream(file.InputStream);

                 
                    Bitmap fullImage =
                        (Bitmap) ImageUtilities.Resize(image, 900, 750, RotateFlipType.RotateNoneFlipNone);
                    ImageUtilities.SaveImage(fullImage, physicalPath, ImageFormat.Jpeg, true);


                 
                    Bitmap thumbNail =
                        (Bitmap) ImageUtilities.Resize(image, 216, 132, RotateFlipType.RotateNoneFlipNone);
                    ImageUtilities.SaveImage(thumbNail, thumbPath, ImageFormat.Jpeg, true);

                }
                else if (Path.GetExtension(file.FileName).ToLower()==(".txt"))
                {
                    FileUtilities.SaveStream(file.InputStream,physicalPath,false);
                }
                else // Must Be Video
                {
                    FileUtilities.SaveStream(file.InputStream, physicalPath, false);

                    thumbPath= thumbPath.Replace(Path.GetExtension(thumbPath), ".jpg");
                    duration = VideoUtilities.GetVideoDuration(physicalPath);

                    VideoUtilities.GetVideoThumbnail(physicalPath,thumbPath);

                }

                UploadedContent uploadedContent = new UploadedContent();
                uploadedContent.MediaGuid = GUID;
                uploadedContent.Type = type;
                uploadedContent.Pictures = new List<string>(2);
                uploadedContent.Duration = duration;

                if (physicalPath != null)
                    uploadedContent.Pictures.Add(ResolvePath(context, physicalPath));
                if (thumbPath != null)
                    uploadedContent.Pictures.Add(ResolvePath(context, thumbPath));



                if (uploadedContent.Pictures.Count > 0)
                    lastSavedFile.Add(uploadedContent);

                
                   context.Session["SavedFileList"] = lastSavedFile;
              
              
                return "Upload Sucessful";
            }
            thumbPath = "";
            physicalPath = "";
       
            return "Failed To Upload File(s)";
        }

    

     

        public static bool CheckFileType(string fileName)
        {

            string ext = Path.GetExtension(fileName);

            switch (ext.ToLower())
            {

                case ".gif":


                    return true;
                case ".png":


                    return true;
                case ".jpg":


                    return true;
                case ".jpeg":


                    return true;
                case ".bmp":


                    return true;
                default:


                    return false;
            }

        }
    }
}