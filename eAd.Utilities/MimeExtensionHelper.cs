  using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
namespace eAd.Utilities
{


public sealed class MimeExtensionHelper
{
    // Fields
    private static MimeTypeCollection _extensionTypes = null;

    // Methods
    private MimeExtensionHelper()
    {
    }

    public static string FindExtension(string mimeType)
    {
        return ExtensionTypes.GetExtension(mimeType);
    }

    public static string FindMime(string file, bool verifyFromContent)
    {
        string extension = Path.GetExtension(file);
        string mimeType = string.Empty;
        try
        {
            if (!string.IsNullOrEmpty(extension))
            {
                mimeType = ExtensionTypes.GetMimeType(extension);
            }
            if (verifyFromContent || (string.IsNullOrEmpty(mimeType) && File.Exists(file)))
            {
                mimeType = FindMimeByContent(file, mimeType);
            }
        }
        catch
        {
        }
        return (mimeType ?? string.Empty).Trim();
    }

    public static string FindMimeByContent(string file, string proposedType)
    {
        FileInfo info = new FileInfo(file);
        if (!info.Exists)
        {
            throw new FileNotFoundException(file);
        }
        byte[] buffer = new byte[Math.Min(0x1000L, info.Length)];
        using (FileStream stream = File.OpenRead(file))
        {
            stream.Read(buffer, 0, buffer.Length);
        }
        return FindMimeByData(buffer, proposedType);
    }

    public static string FindMimeByData(byte[] dataBytes, string mimeProposed)
    {
        if ((dataBytes == null) || (dataBytes.Length == 0))
        {
            throw new ArgumentNullException("dataBytes");
        }
        string str = string.Empty;
        IntPtr zero = IntPtr.Zero;
        if (!string.IsNullOrEmpty(mimeProposed))
        {
            str = mimeProposed;
        }
        int errorCode = FindMimeFromData(IntPtr.Zero, null, dataBytes, dataBytes.Length, string.IsNullOrEmpty(mimeProposed) ? null : mimeProposed, 0, out zero, 0);
        if (errorCode != 0)
        {
            throw Marshal.GetExceptionForHR(errorCode);
        }
        if (zero != IntPtr.Zero)
        {
            str = Marshal.PtrToStringUni(zero);
            Marshal.FreeCoTaskMem(zero);
        }
        return str;
    }

    [DllImport("urlmon.dll", CharSet=CharSet.Unicode, SetLastError=true, ExactSpelling=true)]
    private static extern int FindMimeFromData(IntPtr pBC, [MarshalAs(UnmanagedType.LPWStr)] string pwzUrl, [MarshalAs(UnmanagedType.LPArray, ArraySubType=UnmanagedType.I1, SizeParamIndex=3)] byte[] pBuffer, int cbSize, [MarshalAs(UnmanagedType.LPWStr)] string pwzMimeProposed, int dwMimeFlags, out IntPtr ppwzMimeOut, int dwReserved);
    public static string GetSimpleType(string path)
    {
        switch (Path.GetExtension(path).ToLower())
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
        }
        return "Unknown";
    }

    // Properties
    private static MimeTypeCollection ExtensionTypes
    {
        get
        {
            if (_extensionTypes == null)
            {
                _extensionTypes = new MimeTypeCollection();
            }
            return _extensionTypes;
        }
    }

    // Nested Types
    [Serializable, XmlRoot(ElementName="mimeTypes")]
    private class MimeTypeCollection : List<MimeTypeInfo>
    {
        // Fields
        private SortedList<string, string> _extensions;
        private SortedList<string, List<string>> _mimes;

        // Methods
        public MimeTypeCollection()
        {
            base.Add(new MimeTypeInfo("application/applixware", new List<string>(new string[] { ".aw" })));
            base.Add(new MimeTypeInfo("application/atom+xml", new List<string>(new string[] { ".atom" })));
            base.Add(new MimeTypeInfo("x-x509-ca-cert", new List<string>(new string[] { ".cer" })));
            try
            {
                using (RegistryKey key = Registry.ClassesRoot)
                {
                    using (RegistryKey key2 = key.OpenSubKey(@"MIME\Database\Content Type"))
                    {
                        string[] subKeyNames = key2.GetSubKeyNames();
                        string str = string.Empty;
                        foreach (string str2 in subKeyNames)
                        {
                            string str3 = (str2 ?? string.Empty).Trim();
                            if (!string.IsNullOrEmpty(str3) && string.IsNullOrEmpty(this.GetExtension(str3)))
                            {
                                string name = @"MIME\Database\Content Type\" + str3;
                                using (RegistryKey key3 = key.OpenSubKey(name))
                                {
                                    str = ((key3.GetValue("Extension") as string) ?? string.Empty).Trim();
                                    if (str.Length > 0)
                                    {
                                        base.Add(new MimeTypeInfo(str3, new List<string>(new string[] { str })));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                string str5 = exception.ToString();
            }
        }

        public string GetExtension(string type)
        {
            this.Init();
            return (this._mimes.ContainsKey(type) ? this._mimes[type][0] : string.Empty);
        }

        public string GetMimeType(string extension)
        {
            this.Init();
            return (this._extensions.ContainsKey(extension) ? this._extensions[extension] : string.Empty);
        }

        private void Init()
        {
            if ((((this._extensions == null) || (this._mimes == null)) || (this._extensions.Count == 0)) || (this._mimes.Count == 0))
            {
                this._extensions = new SortedList<string, string>(StringComparer.OrdinalIgnoreCase);
                this._mimes = new SortedList<string, List<string>>(StringComparer.OrdinalIgnoreCase);
                foreach (MimeTypeInfo info in this)
                {
                    this._mimes.Add(info.MimeType, new List<string>(info.Extensions));
                    foreach (string str in info.Extensions)
                    {
                        if (!this._extensions.ContainsKey(str))
                        {
                            this._extensions.Add(str, info.MimeType);
                        }
                    }
                }
            }
        }

    }
}




// Nested Types
}

