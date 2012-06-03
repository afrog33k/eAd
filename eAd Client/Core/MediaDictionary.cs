namespace ClientApp.Core
{
    using System;
    using System.Collections.Generic;

    public class MediaDictionary
    {
        private List<MediaOption> _options = new List<MediaOption>();

        public void Add(string name, string value)
        {
            MediaOption item = new MediaOption {
                Name = name,
                Value = value
            };
            this._options.Add(item);
        }

        public void Clear()
        {
            this._options.Clear();
        }

        public string Get(string name)
        {
            foreach (MediaOption option in this._options)
            {
                if (option.Name == name)
                {
                    return option.Value;
                }
            }
            throw new IndexOutOfRangeException("No such option");
        }

        public string Get(string name, string def)
        {
            foreach (MediaOption option in this._options)
            {
                if (option.Name == name)
                {
                    return option.Value;
                }
            }
            return def;
        }

        public int Count
        {
            get
            {
                return this._options.Count;
            }
        }
    }
}

