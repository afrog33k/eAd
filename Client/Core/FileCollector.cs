using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.IO;
using System.Xml;
using System.Diagnostics;
using Client.Properties;
using Client.Service;
using eAd.DataViewModels;

namespace Client.Core
{
class FileCollector
{
    private CacheManager _cacheManager;
    private RequiredFiles _requiredFiles;


    public RequiredFiles RequiredFiles
    {
        get
        {
            return _requiredFiles;
        }
        set
        {
            _requiredFiles = value;
        }
    }

    public FileCollector(CacheManager cacheManager, FilesModel xmlString)
    {
        _cacheManager = cacheManager;


        // Create a required files object
        _requiredFiles = new RequiredFiles();


        foreach(var item in xmlString.Items )
        {
            _requiredFiles.Files.Add(item);
        }


        // Get the key for later use
        hardwareKey = new HardwareKey();

        // Make a new filelist collection
        _files = new Collection<RequiredFileModel>();

        // Create a webservice call
        xmdsFile =  new ServiceClient();

        // Start up the Xmds Service Object
        //xmdsFile.Credentials = null;
        //xmdsFile.Url = Properties.Settings.Default.Client_xmds_xmds;
        //xmdsFile.UseDefaultCredentials = false;

        // Hook onto the xmds file complete event
        xmdsFile.GetFileCompleted +=(XmdsFileGetFileCompleted);
    }

    /// <summary>
    /// Compares the xml file list with the files currently in the library
    /// Downloads any missing files
    /// For file types of Layout will fire a LayoutChanged event, giving the filename of the layout changed
    /// </summary>
    public void CompareAndCollect()
    {

        var fileForComparison = new List<RequiredFileModel>((IEnumerable<RequiredFileModel>) _requiredFiles.Files);
        //Inspect each file we have here
        foreach (var file in fileForComparison)
        {

            RequiredFileModel fileList = new RequiredFileModel();

            string path = file.Path;
            if (file.FileType == "layout")
            {
                // Layout

                // Does this file exist?
                if (File.Exists(Settings.Default.LibraryPath + @"\" + path ))
                {
                    // Calculate a MD5 for the current file
                    String md5 = _cacheManager.GetMD5(path );

                    System.Diagnostics.Debug.WriteLine(String.Format("Comparing current MD5 [{0}] with given MD5 [{1}]", md5, file.MD5));

                    // Now we have the md5, compare it to the md5 in the xml
                    if (file.MD5 != md5)
                    {
                        // They are different
                        _cacheManager.Remove(path);

                        //TODO: This might be bad! Delete the old layout as it is wrong
                        try
                        {
                            File.Delete(Settings.Default.LibraryPath + @"\" + path );
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(new LogMessage("CompareAndCollect", "Unable to delete incorrect file because: " + ex.Message));
                        }

                        // Get the file and save it
                        fileList.ChunkOffset = 0;
                        fileList.ChunkSize = 0;
                        fileList.Complete = false;
                        fileList.Downloading = false;
                        fileList.Path = path;
                        fileList.FileType = "layout";
                        fileList.MD5 = file.MD5;
                        fileList.Retrys = 0;

                        _files.Add(fileList);
                    }
                    else
                    {
                        // The MD5 of the current file and the MD5 in RequiredFiles are the same.
                        // Therefore make sure this MD5 is in the CacheManager
                        _cacheManager.Add(path, md5);

                        _requiredFiles.MarkComplete(path, md5);
                    }
                }
                else
                {
                    // No - get the file and save it (no chunks)
                    fileList.ChunkOffset = 0;
                    fileList.ChunkSize = 0;
                    fileList.Complete = false;
                    fileList.Downloading = false;
                    fileList.Path = path;
                    fileList.FileType = "layout";
                    fileList.MD5 = file.MD5;
                    fileList.Retrys = 0;

                    _files.Add(fileList);
                }
            }
            else if (file.FileType== "media")
            {
                // Media

                // Does this media exist?
                if (File.Exists(Settings.Default.LibraryPath + @"\" + path))
                {
                    String md5 = _cacheManager.GetMD5(path);

                    System.Diagnostics.Debug.WriteLine(String.Format("Comparing current MD5 [{0}] with given MD5 [{1}]", md5, file.MD5 ));

                    // MD5 the file to make sure it is the same.
                    if (md5 != file.MD5 )
                    {
                        // File changed
                        _cacheManager.Remove(path);

                        // Delete the old media as it is wrong
                        try
                        {
                            File.Delete(Settings.Default.LibraryPath + @"\" + path);
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(new LogMessage("CompareAndCollect", "Unable to delete incorrect file because: " + ex.Message));
                        }

                        // Add to queue
                        fileList.ChunkOffset = 0;
                        fileList.ChunkSize = 512000;
                        fileList.Complete = false;
                        fileList.Downloading = false;
                        fileList.Path = path;
                        fileList.FileType = "media";
                        fileList.Size = file.Size ;
                        fileList.MD5 = file.MD5;
                        fileList.Retrys = 0;

                        _files.Add(fileList);
                    }
                    else
                    {
                        // The MD5 of the current file and the MD5 in RequiredFiles are the same.
                        // Therefore make sure this MD5 is in the CacheManager
                        _cacheManager.Add(path, md5);

                        //string[] filePart = path.Split('.');
                        _requiredFiles.MarkComplete(path, md5);
                    }
                }
                else
                {
                    // No - Get it (async call - with chunks... through another class?)
                    fileList.ChunkOffset = 0;
                    fileList.ChunkSize = 512000;
                    fileList.Complete = false;
                    fileList.Downloading = false;
                    fileList.Path = path;
                    fileList.FileType = "media";
                    fileList.Size = file.Size;
                    fileList.MD5 = file.MD5;
                    fileList.Retrys = 0;

                    _files.Add(fileList);
                }
            }
            else if (file.FileType == "blacklist")
            {
                // Expect <file type="blacklist"><file id="" /></file>
                var items = (List<string>)file.Other;

                BlackList blackList = new BlackList();

                try
                {
                    blackList.Truncate();
                }
                catch { }

                if (items.Count > 0)
                {
                    blackList.Add(items);

                    blackList.Dispose();
                    blackList = null;
                }

                items = null;
            }
            else
            {
                //Ignore node
            }
        }

        Debug.WriteLine(String.Format("There are {0} files to get", _files.Count.ToString()));

        // Output a list of the files we need to get
        string debugMessage = "";

        foreach (RequiredFileModel fileToGet in _files)
            debugMessage += string.Format("File: {0}, Type: {1}, MD5: {2}. ", fileToGet.Path, fileToGet.FileType, fileToGet.MD5);

        Debug.WriteLine(debugMessage);

        // Report the files files back to XMDS
        _requiredFiles.ReportInventory();

        // Write Required Files
        _requiredFiles.WriteRequiredFiles();

        // Is there anything to get?
        if (_files.Count == 0)
        {
            CollectionComplete();
            return;
        }

        // Start with the first file
        _currentFile = 0;

        // Preload the first filelist
        _currentFileList = _files[_currentFile];

        // Get the first file
        GetFile();
    }

    void XmdsFileGetFileCompleted(object sender, GetFileCompletedEventArgs e)
    {
        Debug.WriteLine("Get File Completed");

        try
        {
            // Success / Failure
            if (e.Error != null)
            {
                // There was an error - what do we do?
                if (e.Cancelled)
                {
                    Debug.WriteLine("The GetFile Request has been cancelled.");
                }
                else
                {
                    Debug.WriteLine("The GetFile Request is still active, cancelling.");

                    // Make sure we cancel the request
                    //    xmdsFile.CancelAsync(e.UserState);
                }

                // Log it
                Trace.WriteLine(String.Format("Error From WebService Get File. File=[{1}], Error=[{0}], Try No [{2}]", e.Error.Message, _currentFileList.Path, _currentFileList.Retrys));

                // Retry?
                //TODO: What if we are disconnected from XMDS?
                if (_currentFileList.Retrys < 5)
                {
                    // Increment the Retrys
                    _currentFileList.Retrys++;

                    // Try again
                    GetFile();
                }
                else
                {
                    // Delete the file
                    try
                    {
                        if (_currentFileList.FileType == "layout")
                            File.Delete(Settings.Default.LibraryPath + @"\" + _currentFileList.Path);
                        else
                            File.Delete(Settings.Default.LibraryPath + @"\" + _currentFileList.Path);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(new LogMessage("xmdsFile_GetFileCompleted", "Unable to delete the failed file: " +
                                                       _currentFileList.Path + " Message: " + ex.Message));
                    }

                    // Removed this blacklist code. Files that are not in the cachemanager will not be played on the client (and therefore
                    // we wont try to play a corrupt / partial file).
                    // If we blacklist here we will never try to get this file again, until the blacklist is cleared in XMDS
                    // Better to just skip it for now, and retry it once we have the required files list again

                    /*// Blacklist this file?
                    string[] mediaPath = _currentFileList.Path.Split('.');
                    string mediaId = mediaPath[0];

                    BlackList blackList = new BlackList();
                    blackList.Add(mediaId, BlackListType.Single, String.Format("Max number of retrys failed. BlackListing for this display. Error: {0}", e.Error.Message));
                    */

                    // Move on
                    _currentFileList.Complete = true;
                    _currentFile++;
                }
            }
            else
            {
                // Set the flag to indicate we have a connection to XMDS
                Settings.Default.XmdsLastConnection = DateTime.Now;

                // What file type were we getting
                if (_currentFileList.FileType == "layout")
                {
                    // Decode this byte[] into a string and stick it in the file.
                    string layoutXml = Encoding.UTF8.GetString(e.Result);


                    // We know it is finished and that we need to write to a file
                    try
                    {
                        string fullPath = Settings.Default.LibraryPath + @"\" + _currentFileList.Path;
                        Utilities.CreateFolder(Path.GetDirectoryName(fullPath));
                        StreamWriter sw = new StreamWriter(File.Open(fullPath, FileMode.Create, FileAccess.Write, FileShare.Read));
                        sw.Write(layoutXml);
                        sw.Close();

                        // This file is complete
                        _currentFileList.Complete = true;
                    }
                    catch (IOException ex)
                    {
                        //What do we do if we cant open the file stream?
                        System.Diagnostics.Debug.WriteLine(ex.Message, "FileCollector - GetFileCompleted");
                    }

                    // Check it
                    String md5sum = _cacheManager.GetMD5(_currentFileList.Path );

                    System.Diagnostics.Debug.WriteLine(String.Format("Comparing MD5 of completed download [{0}] with given MD5 [{1}]", md5sum, _currentFileList.MD5));

                    // TODO: What if the MD5 is different?
                    if (md5sum != _currentFileList.MD5)
                    {
                        // Error
                        System.Diagnostics.Trace.WriteLine(new LogMessage("xmdsFile_GetFileCompleted", String.Format("Incorrect MD5 for file: {0}", _currentFileList.Path)));
                    }
                    else
                    {
                        // Add to the CacheManager
                        _cacheManager.Add(_currentFileList.Path , md5sum);

                        // Report this completion back to XMDS
                        _requiredFiles.MarkComplete(_currentFileList.Path, md5sum);
                        _requiredFiles.ReportInventory();
                    }

                    // Fire a layout complete event
                    LayoutFileChanged(_currentFileList.Path);

                    System.Diagnostics.Trace.WriteLine(String.Format("File downloaded: {0}", _currentFileList.Path), "xmdsFile_GetFileCompleted");

                    _currentFile++;
                }
                else
                {

                    Utilities.CreateFolder(Path.GetDirectoryName(Settings.Default.LibraryPath + @"\" + _currentFileList.Path));
                    // Need to write to the file - in append mode
                    FileStream fs = new FileStream(Settings.Default.LibraryPath + @"\" + _currentFileList.Path, FileMode.Append, FileAccess.Write);

                    fs.Write(e.Result, 0, e.Result.Length);
                    fs.Close();
                    fs.Dispose();

                    // Increment the chunkOffset by the amount we just asked for
                    _currentFileList.ChunkOffset = _currentFileList.ChunkOffset + _currentFileList.ChunkSize;

                    // Has the offset reached the total size?
                    if (_currentFileList.Size > _currentFileList.ChunkOffset)
                    {
                        long remaining = _currentFileList.Size - _currentFileList.ChunkOffset;
                        // There is still more to come
                        if (remaining < _currentFileList.ChunkSize)
                        {
                            // Get the remaining
                            _currentFileList.ChunkSize = remaining;
                        }
                    }
                    else
                    {
                        String md5sum = _cacheManager.GetMD5(_currentFileList.Path);

                        System.Diagnostics.Debug.WriteLine(String.Format("Comparing MD5 of completed download [{0}] with given MD5 [{1}]", md5sum, _currentFileList.MD5));

                        if (md5sum != _currentFileList.MD5)
                        {
                            // We need to get this file again
                            try
                            {
                                File.Delete(Settings.Default.LibraryPath + @"\" + _currentFileList.Path);
                            }
                            catch (Exception ex)
                            {
                                // Unable to delete incorrect file
                                //TODO: Should we black list this file?
                                System.Diagnostics.Debug.WriteLine(ex.Message);
                            }

                            // Reset the chunk offset (otherwise we will try to get this file again - but from the beginning (no so good)
                            _currentFileList.ChunkOffset = 0;

                            System.Diagnostics.Trace.WriteLine(String.Format("Error getting file {0}, HASH failed. Starting again", _currentFileList.Path));
                        }
                        else
                        {
                            // Add the MD5 to the CacheManager
                            _cacheManager.Add(_currentFileList.Path, md5sum);

                            // This file is complete
                            _currentFileList.Complete = true;

                            // Fire the File Complete event
                            MediaFileChanged(_currentFileList.Path);

                            System.Diagnostics.Debug.WriteLine(string.Format("File downloaded: {0}", _currentFileList.Path));

                            // Report this completion back to XMDS
                            //       string[] filePart = _currentFileList.Path.Split('.');
                            _requiredFiles.MarkComplete(_currentFileList.Path, md5sum);
                            _requiredFiles.ReportInventory();

                            // All the file has been recieved. Move on to the next file.
                            _currentFile++;
                        }
                    }
                }

                // Before we get the next file render any waiting events
                System.Windows.Forms.Application.DoEvents();
            }
        }
        catch (Exception ex)
        {
            Trace.WriteLine(new LogMessage("xmdsFile_GetFileCompleted", "Unhanded Exception when processing getFile response: " + ex.Message));

            // TODO: Blacklist the file?

            // Consider this file complete because we couldn't write it....
            _currentFileList.Complete = true;
            _currentFile++;
        }

        // Get the next file
        GetFile();
    }

    /// <summary>
    /// Gets the files contained within FileList
    /// </summary>
    public void GetFile()
    {
        if (_currentFile > (_files.Count - 1))
        {
            System.Diagnostics.Debug.WriteLine(String.Format("Finished Receiving {0} files", _files.Count));

            // Clean up
            _files.Clear();
            xmdsFile.Close();

            // Write Required Files
            _requiredFiles.WriteRequiredFiles();

            // Finished getting this file list
            CollectionComplete();
        }
        else
        {
            // Get the current file into the currentfilelist if the current one is finished
            if (_currentFileList.Complete)
            {
                _currentFileList = _files[_currentFile];
            }

            System.Diagnostics.Debug.WriteLine(String.Format("Getting the file : {0} chunk : {1}", _currentFileList.Path, _currentFileList.ChunkOffset.ToString()));

            // Request the file
            xmdsFile.GetFileAsync(Settings.Default.ServerKey, hardwareKey.Key, _currentFileList.Path, _currentFileList.FileType, (int) _currentFileList.ChunkOffset, (int) _currentFileList.ChunkSize, Settings.Default.Version);

            _currentFileList.Downloading = true;
        }
    }



    private XmlDocument xml;
    private HardwareKey hardwareKey;
    private Collection<RequiredFileModel> _files;
    private int _currentFile;
    private RequiredFileModel _currentFileList;
    private ServiceClient xmdsFile;

    public event LayoutFileChangedDelegate LayoutFileChanged;
    public delegate void LayoutFileChangedDelegate(string layoutPath);

    public event MediaFileChangedDelegate MediaFileChanged;
    public delegate void MediaFileChangedDelegate(string path);

    public event CollectionCompleteDelegate CollectionComplete;
    public delegate void CollectionCompleteDelegate();
}
}
