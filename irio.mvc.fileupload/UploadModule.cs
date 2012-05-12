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
using System.Globalization;
using System.Reflection;
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

    private const string C_MARKER = "multipart/form-data; boundary=";
    private const string B_MARKER = "boundary=";
    //private IFileProcessor _processor;
    private UploadStatus _status;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the current upload status.
    /// </summary>
    public UploadStatus Status
    {
        get
        {
            return _status;
        }
    }

    #endregion

    #region Constructor

    #endregion

    #region IHttpModule Members

    /// <summary>
    /// Initialises the module.
    /// </summary>
    /// <param name="context">Application context.</param>
    public void Init(HttpApplication context)
    {
        context.AuthenticateRequest += Context_AuthenticateRequest;
    }

    /// <summary>
    /// Disposes of the module.
    /// </summary>
    public void Dispose()
    {
    }

    #endregion

    #region Event handlers

    /// <summary>
    /// Called when a new request commences (but after authentication).
    /// Preloads the request header and initialises the form stream.
    ///
    /// We do this after authentication so that the file processor will
    /// have access to the security context if it is required.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="e">Event args.</param>
    private void Context_AuthenticateRequest(object sender, EventArgs e)
    {
        var app = sender as HttpApplication;
        HttpWorkerRequest worker = GetWorkerRequest(app.Context);
        int bufferSize;
        string boundary;
        string ct;
        bool statusPersisted = false;

        bufferSize = UploadManager.Instance.BufferSize;

        ct = worker.GetKnownRequestHeader(HttpWorkerRequest.HeaderContentType);

        // Is this a multi-part form which may contain file uploads?
        if (ct != null &&
                string.Compare(ct, 0, C_MARKER, 0, C_MARKER.Length, true, CultureInfo.InvariantCulture) == 0)
        {
            // Get the content length from the header. Don't use Request.ContentLength as this is cached
            // and we don't want it to be calculated until we're done stripping out the files.
            long length = long.Parse(worker.GetKnownRequestHeader(HttpWorkerRequest.HeaderContentLength));

            if (length > 0)
            {
                if (length/1024 > GetMaxRequestLength(app.Context))
                {
                    // End the request if the maximum request length is exceeded.
                    EndRequestOnRequestLengthExceeded(app.Context.Response);
                    return;
                }

                boundary = "--" + ct.Substring(ct.IndexOf(B_MARKER) + B_MARKER.Length);

                InitStatus(length);

                using (var fs = new FormStream( boundary, app.Request.ContentEncoding))
                {
                    // Set up events
                    fs.FileCompleted += fs_FileCompleted;
                    fs.FileCompletedError += fs_FileCompletedError;
                    fs.FileStarted += fs_FileStarted;

                    byte[] data = null;
                    int read = 0;
                    int counter = 0;


                    Console.WriteLine("Status Key: " + fs.StatusKey);
                    if (worker.GetPreloadedEntityBodyLength() > 0)
                    {
                        // Read the first portion of data from the client
                        data = worker.GetPreloadedEntityBody();

                        fs.Write(data, 0, data.Length);

                        if (!String.IsNullOrEmpty(fs.StatusKey))
                        {
                            if (!statusPersisted) PersistStatus(fs.StatusKey);
                            statusPersisted = true;
                            _status.UpdateBytes(data.Length, fs.FileName, null);
                        }

                        counter = data.Length;
                    }

                    bool disconnected = false;

                    // Read data
                    while (counter < length && worker.IsClientConnected() && !disconnected)
                    {
                        if (counter + bufferSize > length)
                        {
                            bufferSize = (int) length - counter;
                        }

                        data = new byte[bufferSize];
                        read = worker.ReadEntityBody(data, bufferSize);

                        if (read > 0)
                        {
                            counter += read;
                            fs.Write(data, 0, read);

                            if (!String.IsNullOrEmpty(fs.StatusKey))
                            {
                                if (!statusPersisted) PersistStatus(fs.StatusKey);
                                statusPersisted = true;
                                _status.UpdateBytes(counter, fs.FileName, null);
                            }
                        }
                        else
                        {
                            disconnected = true;
                        }
                    }

                    if (!worker.IsClientConnected() || disconnected)
                    {
                        app.Context.Response.End();
                        return;
                    }

                    if (fs.ContentMinusFiles != null)
                    {
                        BindingFlags ba = BindingFlags.Instance | BindingFlags.NonPublic;

                        // Replace the worker process with our own version using reflection
                        var wr = new UploadWorkerRequest(worker, fs.ContentMinusFiles);
                        app.Context.Request.GetType().GetField("_wr", ba).SetValue(app.Context.Request, wr);
                    }

                    // Check that the query key is in the request
                    app.Context.Items[UploadManager.STATUS_KEY] = fs.StatusKey;
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
    private void fs_FileStarted(object sender, string fileName, object identifier, Dictionary<string, string> headerItems)
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
    /// <param name="headerItems"></param>
    /// <param name="ex">The exception that was raised.</param>
    private void fs_FileCompletedError(object sender, string fileName, object identifier,Dictionary<string,string> headerItems, Exception ex)
    {
        _status.ErrorFiles.Add(new UploadedFile(fileName, identifier, headerItems, ex));
    }

    /// <summary>
    /// Adds a file to the completed collection of the status when
    /// the form stream indicates that a file has completed.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="fileName">File name</param>
    /// <param name="identifier">Container identifier.</param>
    /// <param name="headerItems"></param>
    private void fs_FileCompleted(object sender, string fileName, object identifier, Dictionary<string,string> headerItems )
    {
        _status.UploadedFiles.Add(new UploadedFile(fileName, identifier, headerItems));
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets the Http worker request.
    /// </summary>
    /// <param name="context">Http context.</param>
    /// <returns>Worker request.</returns>
    private HttpWorkerRequest GetWorkerRequest(HttpContext context)
    {
        IServiceProvider provider = HttpContext.Current;

        return (HttpWorkerRequest) provider.GetService(typeof (HttpWorkerRequest));
    }

    ///// <summary>
    ///// Gets a new file processor from the upload manager.
    ///// </summary>
    ///// <returns>A file processor.</returns>
    //private IFileProcessor GetProcessor()
    //{
    //    _processor = UploadManager.Instance.GetProcessor();
    //    return _processor;
    //}

    /// <summary>
    /// Initialises the upload status which is held as an application
    /// variable using a unique key.
    /// </summary>
    /// <param name="length">The content length.</param>
    private void InitStatus(long length)
    {
        _status = new UploadStatus(length);
    }

    /// <summary>
    /// Perists the status.
    /// </summary>
    /// <param name="key">The status key.</param>
    private void PersistStatus(string key)
    {
        UploadManager.Instance.SetStatus(_status, key);
    }

    /// <summary>
    /// Gets the maximum request length from the configuration settings.
    /// </summary>
    /// <param name="context">Http context.</param>
    /// <returns>The maximum request length (in kb).</returns>
    private int GetMaxRequestLength(HttpContext context)
    {
        int DEFAULT_MAX = 4096;

        // Look up the config setting
        var config = context.GetSection("system.web/httpRuntime") as HttpRuntimeSection;

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
    private void EndRequestOnRequestLengthExceeded(HttpResponse response)
    {
        response.StatusCode = 400; // Generic 400 error just like ASP.Net
        response.StatusDescription = "Maximum request size exceeded";
        response.Flush();
        response.Close();
    }

    #endregion
}
}