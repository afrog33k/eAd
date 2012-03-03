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
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace irio.mvc.fileupload
{
    /// <summary>
    /// Controller for uploads. Must be placed before all upload controls on the page.
    /// </summary>
    public class DJUploadController : WebControl
    {
        #region Declarations

        internal static string UPLOAD_ID_TAG = "::DJ_UPLOAD_ID::";

        private string DEFAULT_CSS_PATH = "upload_styles";
        private string DEFAULT_IMAGE_PATH = "upload_images";
        private string DEFAULT_JS_PATH = "upload_scripts";
        private bool _showProgressBar = true;
        private UploadStatus _status;
        private HiddenField _uploadID;

        #endregion

        /// <summary>
        /// Gets or sets the allowed file extensions (a comma separated list .pdf,.zip,.gif).
        /// </summary>
        /// <value>The allowed file extensions.</value>
        public string AllowedFileExtensions { get; set; }

        /// <summary>
        /// Gets or sets the upload status.
        /// </summary>
        /// <value>The upload status.</value>
        public UploadStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

        /// <summary>
        /// Gets or sets the path to the script file.
        /// </summary>
        /// <value>The script path.</value>
        public string ScriptPath { get; set; }

        /// <summary>
        /// Gets or sets the path to the css file.
        /// </summary>
        /// <value>The image path.</value>
        public string CSSPath { get; set; }

        /// <summary>
        /// Gets or sets the image path.
        /// </summary>
        /// <value>The image path.</value>
        public string ImagePath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the cancel button should be shown.
        /// </summary>
        /// <value><c>true</c> if the cancel button should be shown; otherwise, <c>false</c>.</value>
        public bool ShowCancelButton { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the progress bar should be shown.
        /// </summary>
        /// <value><c>true</c> if the progress bar should be shown; otherwise, <c>false</c>.</value>
        public bool ShowProgressBar
        {
            get { return _showProgressBar; }
            set { _showProgressBar = value; }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (String.IsNullOrEmpty(ImagePath))
            {
                ImagePath = DEFAULT_IMAGE_PATH;
            }

            ImagePath = ImagePath.TrimEnd('/') + "/";

            if (String.IsNullOrEmpty(CSSPath))
            {
                CSSPath = DEFAULT_CSS_PATH;
            }

            CSSPath = CSSPath.TrimEnd('/') + "/";

            if (String.IsNullOrEmpty(ScriptPath))
            {
                ScriptPath = DEFAULT_JS_PATH;
            }

            ScriptPath = ScriptPath.TrimEnd('/') + "/";

            _status = UploadManager.Instance.Status;
            UploadManager.Instance.Status = null;
        }

        /// <summary>
        /// Adds a style sheet reference to the page header.
        /// </summary>
        /// <param name="name">The name of the file to link.</param>
        private void AddStyleLink(string name)
        {
            var link = new HtmlLink();
            link.Attributes.Add("type", "text/css");
            link.Attributes.Add("rel", "stylesheet");
            link.Attributes.Add("href", CSSPath + name);
            Page.Header.Controls.Add(link);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Page.ClientScript.RegisterClientScriptInclude(GetType(), "FU_Script", ScriptPath + "fileupload.js");
            Page.ClientScript.RegisterClientScriptInclude(GetType(), "FU_Script1", ScriptPath + "prototype.js");
            Page.ClientScript.RegisterClientScriptInclude(GetType(), "FU_Script2",
                                                          ScriptPath + "scriptaculous.js?load=effects");
            Page.ClientScript.RegisterClientScriptInclude(GetType(), "FU_Script3", ScriptPath + "modalbox.js");
            Page.ClientScript.RegisterStartupScript(GetType(), "FU_Init",
                                                    "up_initFileUploads('" + ImagePath + "','" +
                                                    (String.IsNullOrEmpty(AllowedFileExtensions)
                                                         ? ""
                                                         : AllowedFileExtensions.ToLower()) + "');", true);

            AddStyleLink("modalbox.css");
            AddStyleLink("uploadstyles.css"); // Always add modalbox.css first as uploadstyles.css has overrides

            EnsureChildControls();

            _uploadID.Value = UPLOAD_ID_TAG + Guid.NewGuid();

            if (ShowProgressBar)
            {
                Page.ClientScript.RegisterOnSubmitStatement(GetType(), "FU_Submit",
                                                            "up_BeginUpload('" + _uploadID.ClientID + "'," +
                                                            (ShowCancelButton ? "true" : "false") + ");");
            }
        }

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            _uploadID = new HiddenField();
            Controls.Add(_uploadID);
        }
    }
}