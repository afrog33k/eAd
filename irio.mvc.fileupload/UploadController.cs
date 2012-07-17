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
using System.Web.UI.WebControls;

namespace irio.mvc.fileupload
{
/// <summary>
/// Controller for uploads. Must be placed before all upload controls on the page.
/// </summary>
public class UploadController : WebControl
{
    internal static string UPLOAD_ID_TAG = "::IRIO_UPLOAD_ID::";
    private UploadStatus _status;
    private string _uploadKey;

    /// <summary>
    /// Gets or sets the upload status.
    /// </summary>
    /// <value>The upload status.</value>
    public UploadStatus Status
    {
        get
        {
            return _status;
        }
        set
        {
            _status = value;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public string UploadKey
    {
        get
        {
            if (string.IsNullOrEmpty(_uploadKey))
            {
                _uploadKey = UPLOAD_ID_TAG + Guid.NewGuid();
            }
            return _uploadKey;
        }
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        _status = UploadManager.Instance.Status;
        UploadManager.Instance.Status = null;
    }

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        //  add the hidden field
        var uploadID = new HiddenField();
        uploadID.Value = UploadKey;

        Controls.Add(uploadID);
    }
}
}