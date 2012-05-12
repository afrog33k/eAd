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

namespace irio.mvc.fileupload
{
/// <summary>
/// Implements the IFileProcessor interface as a dummy testing stub.
/// The byte stream is simply discarded.
/// </summary>
public class DummyProcessor : IFileProcessor
{
    #region Declarations

    private string _fileName;
    private Dictionary<string, string> _headerItems;

    #endregion

    #region Constructor

    #endregion

    #region IFileProcessor Members

    /// <summary>
    /// Starts a new file.
    /// </summary>
    /// <param name="fileName">File name.</param>
    /// <param name="contentType">The content type of the file.</param>
    /// <param name="headerItems">A dictionary of items pulled from the header of the field.</param>
    /// <returns>An optional object used to identify the item in the storage container.</returns>
    public object StartNewFile(string fileName, string contentType, Dictionary<string, string> headerItems)
    {
        _fileName = fileName;
        _headerItems = headerItems;
        return null;
    }

    /// <summary>
    /// Writes to the output file.
    /// </summary>
    /// <param name="buffer">Buffer to write from.</param>
    /// <param name="offset">Offset in the buffer to write from.</param>
    /// <param name="count">Count of bytes to write.</param>
    public void Write(byte[] buffer, int offset, int count)
    {
    }

    /// <summary>
    /// Ends current file processing.
    /// </summary>
    public void EndFile()
    {
    }

    /// <summary>
    /// Returns the name of the file that is currently being processed.
    /// Null if there is no file.
    /// </summary>
    /// <returns>The file name.</returns>
    public string GetFileName()
    {
        return _fileName;
    }

    /// <summary>
    /// Returns the container identifier.
    /// </summary>
    /// <returns>The container identifier.</returns>
    public object GetIdentifier()
    {
        return null;
    }

    /// <summary>
    /// Gets the header items.
    /// </summary>
    public Dictionary<string, string> GetHeaderItems()
    {
        return _headerItems;
    }

    /// <summary>
    /// Dispose of the object.
    /// </summary>
    void IDisposable.Dispose()
    {
    }

    #endregion
}
}