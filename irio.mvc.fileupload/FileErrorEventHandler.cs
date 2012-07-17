using System;
using System.Collections.Generic;

namespace irio.mvc.fileupload
{
    /// <summary>
    /// Delegate for file error events.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="fileName">File name.</param>
    /// <param name="identifier">
    /// An optional identifier passed from the processor and 
    /// used to identify the item in the storage container.
    /// </param>
    /// <param name="ex">The exception that was raised.</param>
    public delegate void FileErrorEventHandler(object sender, string fileName, object identifier, Exception ex);
}