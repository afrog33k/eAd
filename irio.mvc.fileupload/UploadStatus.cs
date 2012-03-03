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
using System.IO;
using System.Xml;

namespace irio.mvc.fileupload
{
    /// <summary>
    /// Contains the status information for an upload request and
    /// allows serialization as as XML message for passing to the
    /// client.
    /// </summary>
    public class UploadStatus
    {
        #region Declarations

        public static string EmptyStatus = "<status empty='true'></status>";
        private readonly DateTime _startTime;
        private readonly long _totalSize;
        private long _bytesSoFar;
        private string _currentFile;
        private object _identifier;
        private int _progressPercent;
        private double _timeInSeconds;

        #endregion

        #region Properties

        /// <summary>
        /// Gets/sets the list of uploaded files.
        /// </summary>
        public List<UploadedFile> UploadedFiles { get; internal set; }

        /// <summary>
        /// Gets/sets the list of files which could not be uploaded due to an error.
        /// </summary>
        public List<UploadedFile> ErrorFiles { get; internal set; }

        /// <summary>
        /// Gets the progress percentage.
        /// </summary>
        public int ProgressPercent
        {
            get { return _progressPercent; }
        }

        /// <summary>
        /// Gets the file that is currently being transferred.
        /// </summary>
        public string CurrentFile
        {
            get { return _currentFile; }
        }

        /// <summary>
        /// Gets the container identifier of the current file.
        /// </summary>
        public object CurrentFileIdentifier
        {
            get { return _identifier; }
        }

        /// <summary>
        /// Gets the total transfer size.
        /// </summary>
        public double TotalSize
        {
            get { return _totalSize; }
        }

        /// <summary>
        /// Gets the total bytes transferred so far.
        /// </summary>
        public long BytesSoFar
        {
            get { return _bytesSoFar; }
        }

        /// <summary>
        /// Gets the time in seconds.
        /// </summary>
        public double TimeInSeconds
        {
            get { return _timeInSeconds; }
        }

        /// <summary>
        /// Gets the time the upload started.
        /// </summary>
        public DateTime StartTime
        {
            get { return _startTime; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the file being processed.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="identifier">Container identifier.</param>
        public void UpdateFile(string fileName, object identifier)
        {
            _currentFile = fileName;
            _identifier = identifier;
        }

        /// <summary>
        /// Updates the byte count.
        /// </summary>
        /// <param name="bytes">New byte count.</param>
        /// <param name="fileName">The name of the current file.</param>
        /// <param name="identifier">The container ID.</param>
        public void UpdateBytes(long bytes, string fileName, object identifier)
        {
            TimeSpan time;
            _currentFile = fileName;
            _identifier = identifier;
            _bytesSoFar = bytes;
            _progressPercent = 100 - (int) (100*((_totalSize - (double) _bytesSoFar)/_totalSize));
            if (_progressPercent < 0) _progressPercent = 0;
            if (_progressPercent > 100) _progressPercent = 100;
            time = DateTime.Now - _startTime;
            _timeInSeconds = time.TotalSeconds;
        }

        /// <summary>
        /// Adds an attribute to an XML node.
        /// </summary>
        /// <param name="doc">XML document.</param>
        /// <param name="node">Node to add to.</param>
        /// <param name="name">Attribute name.</param>
        /// <param name="value">Attribute value.</param>
        private void AddAttribute(XmlDocument doc, XmlNode node, string name, string value)
        {
            XmlAttribute att = doc.CreateAttribute(name);
            att.Value = value;
            node.Attributes.Append(att);
        }

        /// <summary>
        /// Serializes key properties of the object into an XML document.
        /// </summary>
        /// <returns>The serialized XML document.</returns>
        public XmlDocument Serialize()
        {
            var res = new XmlDocument();
            res.LoadXml("<status></status>");
            AddAttribute(res, res.DocumentElement, "progress", _progressPercent.ToString());
            AddAttribute(res, res.DocumentElement, "size", _totalSize.ToString());
            AddAttribute(res, res.DocumentElement, "bytes", _bytesSoFar.ToString());
            AddAttribute(res, res.DocumentElement, "file", Path.GetFileName(_currentFile));
            AddAttribute(res, res.DocumentElement, "containerid",
                         _identifier == null ? String.Empty : _identifier.ToString());

            return res;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="requestSize">The size of the request.</param>
        public UploadStatus(long requestSize)
        {
            UploadedFiles = new List<UploadedFile>();
            ErrorFiles = new List<UploadedFile>();
            _progressPercent = 0;
            _currentFile = String.Empty;
            _bytesSoFar = 0;
            _totalSize = 0;
            _timeInSeconds = 0;
            _startTime = DateTime.Now;
            _totalSize = requestSize;
        }

        #endregion
    }
}