#region License

/*

File Upload HTTP module for ASP.Net (v 2.0)
Copyright (C) 2007-2008 Darren Johnstone (http://darrenjohnstone.com)

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA

*/

#endregion

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace irio.mvc.fileupload
{
/// <summary>
/// Http handler which can process very large file uploads
/// by passing the request stream to a FormStream instance
/// for persistance by an IFileProcessor implementation.
/// </summary>
public class UploadModule : IHttpModule
{
    #region Declarations

    const string C_MARKER = "multipart/form-data; boundary=";
    const string B_MARKER = "boundary=";
    UploadStatus _status;
    IFileProcessor _processor;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the current upload status.
    /// </summary>
    public UploadStatus Status
    {
        get { return _status; }
    }

    #endregion

    #region Constructor

    /// <summary>
    /// Constructor.
    /// </summary>
    public UploadModule()
    {
    }

    #endregion

    #region IHttpModule Members

    /// <summary>
    /// Initialises the module.
    /// </summary>
    /// <param name="context">Application context.</param>
    public void Init(HttpApplication context)
    {
        context.BeginRequest += new EventHandler(Context_AuthenticateRequest);
        context.AddOnBeginRequestAsync(AsyncBeginRequest,AsyncBeginRequestEnd);
    }

    private void AsyncBeginRequestEnd(IAsyncResult ar)
    {
        

    }

    private IAsyncResult AsyncBeginRequest(object sender, EventArgs e, AsyncCallback cb, object extradata)
    {
        Context_AuthenticateRequest(sender, e);
        IAsyncResult result = null;
        return result;
    }

    /// <summary>
    /// Disposes of the module.
    /// </summary>
    public void Dispose()
    {
    }

    #endregion

    #region Event handlers

    ///// <summary>
    ///// Called when a new request commences (but after authentication).
    ///// Preloads the request header and initialises the form stream.
    ///// 
    ///// We do this after authentication so that the file processor will
    ///// have access to the security context if it is required.
    ///// </summary>
    ///// <param name="sender">Sender.</param>
    ///// <param name="e">Event args.</param>
    //void Context_AuthenticateRequest(object sender, EventArgs e)
    //{
    //    HttpApplication app = sender as HttpApplication;
    //    HttpWorkerRequest worker = GetWorkerRequest(app.Context);
    //    int bufferSize;
    //    string boundary;
    //    string ct;
    //    bool statusPersisted = false;

    //    bufferSize = UploadManager.Instance.BufferSize;

    //    ct = worker.GetKnownRequestHeader(HttpWorkerRequest.HeaderContentType);

    //    // Is this a multi-part form which may contain file uploads?
    //    if (ct != null && string.Compare(ct, 0, C_MARKER, 0, C_MARKER.Length, true, CultureInfo.InvariantCulture) == 0)
    //    {
    //        // Get the content length from the header. Don't use Request.ContentLength as this is cached
    //        // and we don't want it to be calculated until we're done stripping out the files.
    //        long length = long.Parse(worker.GetKnownRequestHeader(HttpWorkerRequest.HeaderContentLength));

    //        if (length > 0)
    //        {
    //            if (length / 1024 > GetMaxRequestLength(app.Context))
    //            {
    //                // End the request if the maximum request length is exceeded.
    //                EndRequestOnRequestLengthExceeded(app.Context.Response);
    //                return;
    //            }

    //            boundary = "--" + ct.Substring(ct.IndexOf(B_MARKER) + B_MARKER.Length);

    //            InitStatus(length);

    //            using (FormStream fs = new FormStream(GetProcessor(), boundary, app.Request.ContentEncoding))
    //            {
    //                // Set up events
    //                fs.FileCompleted += new FileEventHandler(fs_FileCompleted);
    //                fs.FileCompletedError += new FileErrorEventHandler(fs_FileCompletedError);
    //                fs.FileStarted += new FileEventHandler(fs_FileStarted);

    //                byte[] data = null;
    //                int read = 0;
    //                int counter = 0;

    //                if (worker.GetPreloadedEntityBodyLength() > 0)
    //                {
    //                    // Read the first portion of data from the client
    //                    data = worker.GetPreloadedEntityBody();

    //                    fs.Write(data, 0, data.Length);

    //                    if (!String.IsNullOrEmpty(fs.StatusKey))
    //                    {
    //                        if (!statusPersisted) PersistStatus(fs.StatusKey);
    //                        statusPersisted = true;
    //                        _status.UpdateBytes(data.Length, _processor.GetFileName(), _processor.GetIdentifier());
    //                    }

    //                    counter = data.Length;
    //                }

    //                bool disconnected = false;

    //                // Read data    
    //                while (counter < length && worker.IsClientConnected() && !disconnected)
    //                {
    //                    if (counter + bufferSize > length)
    //                    {
    //                        bufferSize = (int)length - counter;
    //                    }

    //                    data = new byte[bufferSize];
    //                    read = worker.ReadEntityBody(data, bufferSize);

    //                    if (read > 0)
    //                    {
    //                        counter += read;
    //                        fs.Write(data, 0, read);

    //                        if (!String.IsNullOrEmpty(fs.StatusKey))
    //                        {
    //                            if (!statusPersisted) PersistStatus(fs.StatusKey);
    //                            statusPersisted = true;
    //                            _status.UpdateBytes(counter, _processor.GetFileName(), _processor.GetIdentifier());
    //                        }
    //                    }
    //                    else
    //                    {
    //                        disconnected = true;
    //                    }
    //                }

    //                if (!worker.IsClientConnected() || disconnected)
    //                {
    //                    app.Context.Response.End();
    //                    return;
    //                }

    //                if (fs.ContentMinusFiles != null)
    //                {
    //                    BindingFlags ba = BindingFlags.Instance | BindingFlags.NonPublic;

    //                    // Replace the worker process with our own version using reflection
    //                    UploadWorkerRequest wr = new UploadWorkerRequest(worker, fs.ContentMinusFiles);
    //                    app.Context.Request.GetType().GetField("_wr", ba).SetValue(app.Context.Request, wr);
    //                }

    //                // Check that the query key is in the request
    //                app.Context.Items[UploadManager.STATUS_KEY] = fs.StatusKey;
    //            }
    //        }

    //    }
    //}

    void Context_AuthenticateRequest(object sender, EventArgs e)
    {
        HttpApplication application = (HttpApplication)sender;
        HttpContext context = application.Context;
        IServiceProvider provider = (IServiceProvider)context;
        HttpWorkerRequest workerRequest = (HttpWorkerRequest)provider.GetService(typeof(HttpWorkerRequest));
      
        var ct = workerRequest.GetKnownRequestHeader(HttpWorkerRequest.HeaderContentType);

        //    // Is this a multi-part form which may contain file uploads?
        if (ct != null && string.Compare(ct, 0, C_MARKER, 0, C_MARKER.Length, true, CultureInfo.InvariantCulture) == 0)
        {
            HttpRequest request = context.Request;
            if (request.Files.Count > 0)
            { 
                foreach (string key in request.Files.Keys)
                {
                    FileStream fs = null;
                    // Check if body contains data
                  //  if (workerRequest.HasEntityBody())
                    {
                        // get the total body length
                     //  int requestLength = workerRequest.GetTotalEntityBodyLength();
                    
                    //    if (!workerRequest.IsEntireEntityBodyIsPreloaded())
                        {
                            byte[] buffer = new byte[512000];
                            string fileName =request.Files[key].FileName;//context.Request.QueryString["fileName"].Split(new char[] { '\\' });
                            int requestLength = request.Files[key].ContentLength;
                            var uploadFolder = ConfigurationManager.AppSettings["TemporaryUploadFolder"];

                            Console.WriteLine("Starting Download " + DateTime.Now.ToString(CultureInfo.InvariantCulture));
                        
                                fs = new FileStream(context.Server.MapPath(uploadFolder + fileName), FileMode.Create);
                            // Set the received bytes to initial bytes before start reading
                            int initialBytes = 0;
                            int receivedBytes = initialBytes;
                            while (requestLength - receivedBytes >= initialBytes)
                            {
                                // Read another set of bytes
                                 initialBytes = request.Files[key].InputStream.Read(buffer,0, buffer.Length);
                          //    request.Files[key].ContentType); //workerRequest.ReadEntityBody(buffer, buffer.Length);
                                // Write the chunks to the physical file
                                fs.Write(buffer, 0, buffer.Length);
                                // Update the received bytes
                                receivedBytes += initialBytes;
                                Console.WriteLine("Saving file " + receivedBytes/requestLength + " bytes downloaded @ " + DateTime.Now.ToString(CultureInfo.InvariantCulture));
                            }
                            initialBytes = workerRequest.ReadEntityBody(buffer, requestLength - receivedBytes);

                        }
                    }
                    fs.Flush();
                    fs.Close();

                   

                }



            }
        }
    }

    /// <summary>
    /// Updates the currently processed file when the file stream indicates
    /// it has started processing a new file.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="fileName">File name.</param>
    /// <param name="identifier">Container identifier.</param>
    void fs_FileStarted(object sender, string fileName, object identifier)
    {
        _status.UpdateFile(fileName, identifier);
    }

    /// <summary>
    /// Adds a file to the error collection of the status when
    /// the form stream indicates that a file has completed in error.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="fileName">File name</param>
    /// <param name="identifier">Container identifier.</param>
    /// <param name="ex">The exception that was raised.</param>
    void fs_FileCompletedError(object sender, string fileName, object identifier, Exception ex)
    {
        _status.ErrorFiles.Add(new UploadedFile(fileName, identifier, _processor.GetHeaderItems(), ex));
    }

    /// <summary>
    /// Adds a file to the completed collection of the status when
    /// the form stream indicates that a file has completed.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="fileName">File name</param>
    /// <param name="identifier">Container identifier.</param>
    void fs_FileCompleted(object sender, string fileName, object identifier)
    {
        _status.UploadedFiles.Add(new UploadedFile(fileName, identifier, _processor.GetHeaderItems()));
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets the Http worker request.
    /// </summary>
    /// <param name="context">Http context.</param>
    /// <returns>Worker request.</returns>
    HttpWorkerRequest GetWorkerRequest(HttpContext context)
    {
        IServiceProvider provider = (IServiceProvider)HttpContext.Current;

        return (HttpWorkerRequest)provider.GetService(typeof(HttpWorkerRequest));
    }

    /// <summary>
    /// Gets a new file processor from the upload manager.
    /// </summary>
    /// <returns>A file processor.</returns>
    IFileProcessor GetProcessor()
    {
        _processor = UploadManager.Instance.GetProcessor();
        return _processor;
    }

    /// <summary>
    /// Initialises the upload status which is held as an application
    /// variable using a unique key.
    /// </summary>
    /// <param name="length">The content length.</param>
    void InitStatus(long length)
    {
        _status = new UploadStatus(length);
    }

    /// <summary>
    /// Perists the status.
    /// </summary>
    /// <param name="key">The status key.</param>
    void PersistStatus(string key)
    {
        UploadManager.Instance.SetStatus(_status, key);
    }

    /// <summary>
    /// Gets the maximum request length from the configuration settings.
    /// </summary>
    /// <param name="context">Http context.</param>
    /// <returns>The maximum request length (in kb).</returns>
    int GetMaxRequestLength(HttpContext context)
    {
        int DEFAULT_MAX = 4096;

        // Look up the config setting
        System.Web.Configuration.HttpRuntimeSection config = context.GetSection("system.web/httpRuntime") as System.Web.Configuration.HttpRuntimeSection;

        if (config == null)
        {
            return DEFAULT_MAX; // None found. Return 4096Kb which is the default setting.
        }
        else
        {
            return config.MaxRequestLength;
        }
    }

    /// <summary>
    /// Ends the request if the maximum request length is exceeded.
    /// </summary>
    /// <param name="response">The HTTP response.</param>
    void EndRequestOnRequestLengthExceeded(HttpResponse response)
    {
        response.StatusCode = 400; // Generic 400 error just like ASP.Net
        response.StatusDescription = "Maximum request size exceeded";
        response.Flush();
        response.Close();
    }

    #endregion
}

}