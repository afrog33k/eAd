namespace ClientApp
{
    using System;
    using System.IO;
    using System.Text;

    internal class TemporaryHtml : IDisposable
    {
        private string _fileContent;
        private string _filePath;
        private string _headContent;
        private string _resourceTemplate;

        public void Dispose()
        {
            File.Delete(this._filePath);
        }

        private void Store()
        {
            this._filePath = System.IO.Path.GetTempFileName();
            using (FileStream stream = new FileStream("Resources/HtmlTemplate.htm", FileMode.OpenOrCreate))
            {
                using (TextReader reader = new StreamReader(stream))
                {
                    this._resourceTemplate = reader.ReadToEnd();
                }
            }
            this._resourceTemplate = this._resourceTemplate.Replace("<!--[[[HEADCONTENT]]]-->", this._headContent);
            this._resourceTemplate = this._resourceTemplate.Replace("<!--[[[BODYCONTENT]]]-->", this._fileContent);
            using (StreamWriter writer = new StreamWriter(File.Open(this._filePath, FileMode.Create, FileAccess.Write, FileShare.Read), Encoding.UTF8))
            {
                writer.Write(this._resourceTemplate);
                writer.Close();
            }
        }

        public string BodyContent
        {
            set
            {
                this._fileContent = value;
                this.Store();
            }
        }

        public string HeadContent
        {
            set
            {
                this._headContent = value;
            }
        }

        public string Path
        {
            get
            {
                return this._filePath;
            }
        }
    }
}

