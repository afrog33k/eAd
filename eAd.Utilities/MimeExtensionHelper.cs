using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Win32;

namespace eAd.Utilities
{
    public sealed class MimeExtensionHelper
    {

        public static string GetSimpleType(string path)
        {
            var ext = Path.GetExtension(path).ToLower();
            switch (ext)
            {
                case ".flv":
                case ".m4v":
                case ".mp4":
                case ".avi":
                    return "Video";
                case ".jpg":
                case ".gif":
                case ".bmp":
                case ".png":
                    return "Image";
                case ".txt":
                    return "Marquee";
                case ".ppt":
                case ".pptx":
                    return "Presentation";
                default:
                    return "Unknown";
            }

        }

        private MimeExtensionHelper() { }

        /// <summary>Finds extension associated with specified mime type</summary>
        /// <param name="mimeType">mime type you search extension for, e.g.: "application/octet-stream"</param>
        /// <returns>most used extension, associated with provided type, e.g.: ".bin"</returns>
        public static string FindExtension(string mimeType)
        {
            return ExtensionTypes.GetExtension(mimeType);
        }

        /// <summary>Finds mime type using provided extension and/or file's binary content.</summary>
        /// <param name="file">Full file path</param>
        /// <param name="verifyFromContent">Should the file's content be examined to verify founded value.</param>
        /// <returns>mime type of file, e.g.: "application/octet-stream"</returns>
        public static string FindMime(string file, bool verifyFromContent)
        {
            string extension = Path.GetExtension(file);
            string mimeType = string.Empty;
            try
            {
                if (!String.IsNullOrEmpty(extension))
                    mimeType = ExtensionTypes.GetMimeType(extension);
                if (verifyFromContent
                        || (String.IsNullOrEmpty(mimeType) && File.Exists(file)))
                    mimeType = FindMimeByContent(file, mimeType);
            }
            catch { }
            return (mimeType ?? string.Empty).Trim();//"application/octet-stream"
        }

        /// <summary>Finds mime type for file using it's binary data.</summary>
        /// <param name="file">Full path to file.</param>
        /// <param name="proposedType">Optional. Expected file's type.</param>
        /// <returns>mime type, e.g.: "application/octet-stream"</returns>
        public static string FindMimeByContent(string file
                , string proposedType)
        {
            FileInfo fi = new FileInfo(file);
            if (!fi.Exists)
                throw new FileNotFoundException(file);
            byte[] buf = new byte[Math.Min(4096L, fi.Length)];
            using (FileStream fs = File.OpenRead(file))
                fs.Read(buf, 0, buf.Length);
            return FindMimeByData(buf, proposedType);
        }

        /// <summary>Finds mime type for binary data.</summary>
        /// <param name="dataBytes">Binary data to examine.</param>
        /// <param name="mimeProposed">Optional. Expected mime type.</param>
        /// <returns>mime type, e.g.: "application/octet-stream"</returns>
        public static string FindMimeByData(byte[] dataBytes, string mimeProposed)
        {
            if (dataBytes == null || dataBytes.Length == 0)
                throw new ArgumentNullException("dataBytes");
            string mimeRet = String.Empty;
            IntPtr outPtr = IntPtr.Zero;
            if (!String.IsNullOrEmpty(mimeProposed))
                mimeRet = mimeProposed;
            int result = FindMimeFromData(IntPtr.Zero
                    , null
                    , dataBytes
                    , dataBytes.Length
                    , String.IsNullOrEmpty(mimeProposed) ? null : mimeProposed
                    , 0
                    , out outPtr
                    , 0);
            if (result != 0)
                throw Marshal.GetExceptionForHR(result);
            if (outPtr != null && outPtr != IntPtr.Zero)
            {
                mimeRet = Marshal.PtrToStringUni(outPtr);
                Marshal.FreeCoTaskMem(outPtr);
            }
            return mimeRet;
        }

        [DllImport("urlmon.dll"
                , CharSet = CharSet.Unicode
                , ExactSpelling = true
                , SetLastError = true)]
        static extern Int32 FindMimeFromData(IntPtr pBC
                , [MarshalAs(UnmanagedType.LPWStr)] String pwzUrl
                 , [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I1, SizeParamIndex = 3)] Byte[] pBuffer
                  , Int32 cbSize
                  , [MarshalAs(UnmanagedType.LPWStr)] String pwzMimeProposed
                   , Int32 dwMimeFlags
                   , out IntPtr ppwzMimeOut
                   , Int32 dwReserved);

        private static MimeTypeCollection _extensionTypes = null;
        private static MimeTypeCollection ExtensionTypes
        {
            get
            {
                if (_extensionTypes == null)
                    _extensionTypes = new MimeTypeCollection();
                return _extensionTypes;
            }
        }

        [Serializable]
        [XmlRoot(ElementName = "mimeTypes")]
        private class MimeTypeCollection : List<MimeTypeCollection.MimeTypeInfo>
        {
            private SortedList<string, string> _extensions;
            private SortedList<string, List<string>> _mimes;

            private void Init()
            {
                if (_extensions == null || _mimes == null
                        || _extensions.Count == 0 || _mimes.Count == 0)
                {
                    _extensions = new SortedList<string, string>(StringComparer.OrdinalIgnoreCase);
                    _mimes = new SortedList<string, List<string>>(StringComparer.OrdinalIgnoreCase);
                    foreach (var mime in this)
                    {
                        _mimes.Add(mime.MimeType, new List<string>(mime.Extensions));
                        foreach (string ext in mime.Extensions)
                            if (!_extensions.ContainsKey(ext))
                                _extensions.Add(ext, mime.MimeType);
                    }
                }
            }

            public String GetExtension(string type)
            {
                Init();
                return _mimes.ContainsKey(type) ? _mimes[type][0] : string.Empty;
            }

            public String GetMimeType(string extension)
            {
                Init();
                return _extensions.ContainsKey(extension) ? _extensions[extension] : string.Empty;
            }

            public MimeTypeCollection()
            {
                this.Add(new MimeTypeInfo("application/applixware", new List<string>(new[] { ".aw" })));
                this.Add(new MimeTypeInfo("application/atom+xml", new List<string>(new[] { ".atom" })));
                // ... Whole list from apache's site
                this.Add(new MimeTypeInfo("x-x509-ca-cert", new List<string>(new[] { ".cer" })));
                try
                {
                    using (RegistryKey classesRoot = Registry.ClassesRoot)
                    using (RegistryKey typeKey = classesRoot.OpenSubKey(@"MIME\Database\Content Type"))
                    {
                        string[] subKeyNames = typeKey.GetSubKeyNames();
                        string extension = string.Empty;
                        foreach (string keyname in subKeyNames)
                        {
                            string trimmed = (keyname ?? string.Empty).Trim();
                            if (string.IsNullOrEmpty(trimmed))
                                continue;
                            if (!String.IsNullOrEmpty(GetExtension(trimmed)))
                                continue;
                            string subKey = "MIME\\Database\\Content Type\\" + trimmed;
                            using (RegistryKey curKey = classesRoot.OpenSubKey(subKey))
                            {
                                extension = (curKey.GetValue("Extension") as string ?? string.Empty).Trim();
                                if (extension.Length > 0)
                                    this.Add(new MimeTypeInfo(trimmed
                                            , new List<string>(new[] { extension })));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string s = ex.ToString();
                }
            }

            [Serializable]
            public class MimeTypeInfo
            {
                [XmlAttribute(AttributeName = "mimeType")]
                public String MimeType { get; set; }

                [XmlElement("extension")]
                public List<String> Extensions { get; set; }

                public MimeTypeInfo(string mimeType, List<string> extensions)
                {
                    MimeType = mimeType;
                    Extensions = extensions;
                }

                public MimeTypeInfo() { }
            }
        }
    }

}



