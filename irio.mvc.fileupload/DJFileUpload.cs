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
using System.Web.UI;
using System.Web.UI.WebControls;

namespace irio.mvc.fileupload
{
    /// <summary>
    /// Multiple file upload control with progress bar.
    /// </summary>
    public class DJFileUpload : WebControl, INamingContainer
    {
        #region Declarations

        private int DEFAULT_INITIAL = 1;
        private int DEFAULT_MAXIMUM = 5;
        private DJUploadController _controller;

        #endregion

        /// <summary>
        /// Gets or sets the initial number of file uploads.
        /// </summary>
        /// <value>Gets or sets the initial number of file uploads.</value>
        public int InitialFileUploads { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of file uploads.
        /// </summary>
        /// <value>Gets or sets the maximum number of file uploads.</value>
        public int MaxFileUploads { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the upload button should be shown.
        /// </summary>
        /// <value><c>true</c> if the upload button should be shown; otherwise, <c>false</c>.</value>
        public bool ShowUploadButton { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the add button should be shown.
        /// </summary>
        /// <value><c>true</c> if the add button should be shown; otherwise, <c>false</c>.</value>
        public bool ShowAddButton { get; set; }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (InitialFileUploads <= 0)
            {
                InitialFileUploads = DEFAULT_INITIAL;
            }

            if (MaxFileUploads <= 0)
            {
                MaxFileUploads = DEFAULT_MAXIMUM;
            }
        }

        /// <summary>
        /// Gets the upload controller.
        /// </summary>
        /// <returns>The upload controller.</returns>
        private DJUploadController GetController()
        {
            DJUploadController res = null;

            foreach (object o in Page.Form.Controls)
            {
                res = o as DJUploadController;

                if (res != null)
                    break;
            }

            if (res == null)
            {
                throw new Exception(
                    "An instance of the DJUploadController control must be placed at the beginning of the page before other controls.");
            }

            return res;
        }

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use 
        /// composition-based implementation to create any child controls they contain 
        /// in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            _controller = GetController();

            // Create the container
            var outerContainer = new Panel();
            outerContainer.CssClass = "upUploadBox";
            Controls.Add(outerContainer);

            // Create the file uploads
            for (int i = 0; i < MaxFileUploads; i++)
            {
                var fuContainer = new Panel();
                fuContainer.CssClass = "upFileInputs";
                outerContainer.Controls.Add(fuContainer);


                var fu = new FileUpload();
                fu.CssClass = "upFile";
                fuContainer.Controls.Add(fu);

                var btnRemove = new ImageButton();
                fuContainer.Controls.Add(btnRemove);
                btnRemove.AlternateText = "Remove upload";
                btnRemove.ImageUrl = _controller.ImagePath + "removebutton.gif";
                btnRemove.OnClientClick = "up_RemoveUpload(this); return false;";

                if (i >= InitialFileUploads)
                {
                    fuContainer.Style.Add("display", "none");
                }
            }

            // Create the buttons
            var btnGo = new ImageButton();
            outerContainer.Controls.Add(btnGo);
            btnGo.AlternateText = "Upload now";
            btnGo.ImageUrl = _controller.ImagePath + "uploadbutton.gif";
            btnGo.Visible = ShowUploadButton;

            var btnAdd = new ImageButton();
            outerContainer.Controls.Add(btnAdd);
            btnAdd.AlternateText = "Add a new upload";
            btnAdd.ImageUrl = _controller.ImagePath + "addbutton.gif";
            btnAdd.OnClientClick = "up_AddUpload('" + ClientID + "'); return false;";
            btnAdd.Visible = ShowAddButton;
        }
    }
}