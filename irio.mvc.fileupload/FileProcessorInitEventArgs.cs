using System;

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
            get
            {
                return _processor;
            }
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
}