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
using System.Web;

namespace irio.mvc.fileupload
{
    /// <summary>
    /// Event arguments for the ProcessorInit event.
    /// </summary>
    public class FileProcessorInitEventArgs : EventArgs
    {
        #region Declarations

        private readonly IFileProcessor _processor;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the file processor.
        /// </summary>
        public IFileProcessor Processor
        {
            get { return _processor; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="processor">File processor instance.</param>
        public FileProcessorInitEventArgs(IFileProcessor processor)
        {
            _processor = processor;
        }

        #endregion
    }

    /// <summary>
    /// Delegate for the ProcessorInit event.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="args">Event args.</param>
    public delegate void FileProcessorInitEventHandler(object sender, FileProcessorInitEventArgs args);

    /// <summary>
    /// Manages uploads and acts as a factory class for file processors.
    /// </summary>
    public sealed class UploadManager
    {
        #region Declarations

        private const int MIN_BUFFER_SIZE = 1024;
        private const int DEF_BUFFER_SIZE = 1024*128;

        public const string STATUS_KEY = "DJUploadStatus";
        private static UploadManager _instance;
        private static readonly object _padlock = new object();
        private int _bufferSize;
        private Type _processorType;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        private UploadManager()
        {
            // Set up the default processor.
            _processorType = typeof (FileSystemProcessor);
            _bufferSize = DEF_BUFFER_SIZE;
        }

        #endregion

        #region Events

        /// <summary>
        /// Fired when a processor is initialised but before it is used.
        /// Set processor properties here.
        /// </summary>
        public event FileProcessorInitEventHandler ProcessorInit;

        /// <summary>
        /// Fires the ProcessorInit event.
        /// </summary>
        /// <param name="processor">File processor.</param>
        public void OnProcessorInit(IFileProcessor processor)
        {
            if (ProcessorInit != null)
                ProcessorInit(this, new FileProcessorInitEventArgs(processor));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current upload status.
        /// </summary>
        public UploadStatus Status
        {
            get
            {
                string key;

                key = HttpContext.Current.Request.QueryString[STATUS_KEY];
                if (key == null)
                {
                    key = (string) HttpContext.Current.Items[STATUS_KEY];
                    if (key == null)
                        return null;
                }

                return HttpContext.Current.Application[STATUS_KEY + key] as UploadStatus;
            }
            internal set
            {
                string key;

                key = HttpContext.Current.Request.QueryString[STATUS_KEY];
                if (key != null)
                {
                    SetStatus(value, key);
                }
            }
        }

        public static UploadStatus GetStatus(string id)
        {
            return HttpContext.Current.Application[STATUS_KEY + id] as UploadStatus;
        }
        /// <summary>
        /// Gets/sets the buffer size for reading from the request stream.
        /// </summary>
        public int BufferSize
        {
            get { return _bufferSize; }
            set
            {
                if (_bufferSize <= MIN_BUFFER_SIZE)
                    throw new ArgumentException("Minimum buffer size violation");
                _bufferSize = value;
            }
        }

        /// <summary>
        /// Gets the singleton instance in a thread safe manner.
        /// </summary>
        public static UploadManager Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new UploadManager();
                    }
                    return _instance;
                }
            }
        }

        /// <summary>
        /// Gets/sets the processor type (must implement IFileProcessor).
        /// </summary>
        public Type ProcessorType
        {
            get { return _processorType; }
            set
            {
                if (value == null || value.GetInterface("IFileProcessor", false) == null)
                    throw new ArgumentException("File processor must implement IFileProcessor");
                _processorType = value;
            }
        }

        /// <summary>
        /// Sets the upload status.
        /// </summary>
        /// <param name="status">Status to set.</param>
        /// <param name="key">Upload key.</param>
        internal void SetStatus(UploadStatus status, string key)
        {
            HttpContext.Current.Application[STATUS_KEY + key] = status;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Factory method creates a new instance of IFileProcessor.
        /// </summary>
        /// <returns>The created file processor.</returns>
        public IFileProcessor GetProcessor()
        {
            IFileProcessor processor;

            processor = (IFileProcessor) Activator.CreateInstance(_processorType);
            OnProcessorInit(processor);
            return processor;
        }

        #endregion
    }
}