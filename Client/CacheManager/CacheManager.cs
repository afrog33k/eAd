

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Xml;
using Client.Core;
using Client.Properties;
using eAd.DataViewModels;
using eAd.Utilities;

namespace Client
{
    public class CacheManager
    {
        public Collection<Md5Resource> Files;

        public CacheManager()
        {
            Files = new Collection<Md5Resource>();
        }

        /// <summary>
        /// Gets the MD5 for the given path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public String GetMD5(String path)
        {
            // Either we already have the MD5 stored
            foreach (Md5Resource file in Files)
            {
                if (file.Path == path)
                {
                    // Check to see if this file has been modified since the MD5 cache
                    DateTime lastWrite = File.GetLastWriteTime(Settings.Default.LibraryPath + @"\" + path);

                    if (lastWrite > file.CacheDate)
                    {
                        Debug.WriteLine(new LogMessage("GetMD5", "File has been written to since cache, recalculating"), LogType.Info.ToString());

                        // Get the MD5 again
                        String md5 = CalcMD5(path);

                        // Store the new cacheDate AND the new MD5
                        Remove(path);

                        Add(path, md5);
                        
                        // Return the new MD5
                        return md5;
                    }

                    return file.MD5;
                }
            }

            return CalcMD5(path);
        }

        /// <summary>
        /// Calculates the MD5 for a path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string CalcMD5(String path)
        {
            try
            {
                // Open the file and get the MD5
                using (FileStream md5Fs = new FileStream(Settings.Default.LibraryPath + @"\" + path, FileMode.Open, FileAccess.Read))
                {
                    return Hashes.MD5(md5Fs);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(new LogMessage("CalcMD5", "Unable to calc the MD5 because: " + ex.Message), LogType.Error.ToString());
                
                // Return a 0 MD5 which will immediately invalidate the file
                return "0";
            }
        }

        /// <summary>
        /// Adds a MD5 to the CacheManager
        /// </summary>
        /// <param name="path"></param>
        /// <param name="md5"></param>
        public void Add(String path, String md5)
        {
            // First check to see if this path is in the collection
            foreach (Md5Resource file in Files)
            {
                if (file.Path == path)
                    return;
            }

            // We need to generate the MD5 and store it for later
            Md5Resource md5Resource = new Md5Resource();

            md5Resource.Path = path;
            md5Resource.MD5 = md5;
            md5Resource.CacheDate = DateTime.Now;

            // Add the resource to the collection
            Files.Add(md5Resource);

            System.Diagnostics.Debug.WriteLine(new LogMessage("Add", "Adding new MD5 to CacheManager"), LogType.Info.ToString());
        }

        /// <summary>
        /// Removes the MD5 resource associated with the Path given
        /// </summary>
        /// <param name="path"></param>
        public void Remove(String path)
        {
            // Loop through all MD5s and remove any that match the path
            for (int i = 0; i < Files.Count; i++)
            {
                Md5Resource file = Files[i];

                if (file.Path == path)
                {
                    Files.Remove(file);

                    Debug.WriteLine(new LogMessage("Remove", "Removing stale MD5 from the CacheManager"), LogType.Info.ToString());
                }
            }
        }

        /// <summary>
        /// Writes the CacheManager to disk
        /// </summary>
        public void WriteCacheManager()
        {
            Debug.WriteLine(new LogMessage("CacheManager - WriteCacheManager", "About to Write the Cache Manager"), LogType.Info.ToString());

            try
            {
                using (StreamWriter streamWriter = new StreamWriter(App.UserAppDataPath + "\\" + Settings.Default.CacheManagerFile))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(CacheManager));

                    xmlSerializer.Serialize(streamWriter, this);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(new LogMessage("MainForm_FormClosing", "Unable to write CacheManager to disk because: " + ex.Message));
            }
        }

        /// <summary>
        /// Is the given URI a valid file?
        /// </summary>
        /// <param name="path"></param>
        /// <returns>True is it is and false if it isnt</returns>
        public bool IsValidPath(String path)
        {
            // TODO: what makes a path valid?
            // Currently a path is valid if it is in the cache
            if (String.IsNullOrEmpty(path))
                return false;

            // Search for this path
            foreach (Md5Resource file in Files)
            {
                if (file.Path == path)
                {
                    // If we cached it over 2 minutes ago, then check the GetLastWriteTime
                    if (file.CacheDate > DateTime.Now.AddMinutes(-2))
                        return true;

                    try
                    {
                        // Check to see if this file has been modified since the MD5 cache
                        // If it has then we assume invalid, otherwise its valid
                        DateTime lastWrite = File.GetLastWriteTime(Settings.Default.LibraryPath + @"\" + path);

                        if (lastWrite <= file.CacheDate)
                            return true;
                        else
                            return false;
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(new LogMessage("IsValid", "Unable to determine if the file is valid. Assuming not valid: " + ex.Message), LogType.Error.ToString());

                        // Assume invalid
                        return false;
                    }
                }
            }

            // Reached the end of the cache and havent found the file.
            return false;
        }

        /// <summary>
        /// Is the provided layout file a valid layout (has all media)
        /// </summary>
        /// <param name="layoutFile"></param>
        /// <returns></returns>
        public bool IsValidLayout(string layoutFile)
        {
            layoutFile = "Layouts\\" + layoutFile;
            Debug.WriteLine("Checking if Layout " + layoutFile + " is valid");

            if (!IsValidPath(layoutFile))
                return false;

            // Load the XLF, get all media ID's
            using(var stream = new FileStream(Settings.Default.LibraryPath + @"\" +layoutFile,FileMode.Open))
            {
              var   serializer = new XmlSerializer(typeof (LayoutModel));
              var   layout = (LayoutModel)serializer.Deserialize(stream);

                foreach (var regions in layout.Regions)
                {
                    foreach (var media in regions.Media)
                    {
                        // Is this a stored media type?
                        switch (media.Type)
                        {
                            case "Video":
                            case "Image":
                            case "Flash":
                            case "Ppt":
                                
                                // Get the path and see if its valid
                                if (!IsValidPath(media.Options.Uri))
                                {
                                    Debug.WriteLine("Invalid Media: " + media.Id.ToString());
                                    return false;
                                }

                                break;

                            default:
                                continue;
                        }
                    }
                }

            }

            Debug.WriteLine("Layout " + layoutFile + " is valid");

            return true;
        }

        /// <summary>
        /// Regenerate from Required Files
        /// </summary>
        public void Regenerate()
        {
            if (!File.Exists(App.UserAppDataPath + "\\" + Settings.Default.RequiredFilesFile))
                return;

            // Open the XML file and check each required file that isnt already there
            XmlDocument xml = new XmlDocument();
            xml.Load(App.UserAppDataPath + "\\" + Settings.Default.RequiredFilesFile);

            XmlNodeList fileNodes = xml.SelectNodes("//RequiredFileModel/Path");

            foreach (XmlNode file in fileNodes)
            {
                string path = file.InnerText;

                // Does the file exist?
                if (!File.Exists(Settings.Default.LibraryPath + @"\" + path))
                    continue;

                // Add this file to the cache manager
                Add(path, GetMD5(path));
            }
        }
    }
}
