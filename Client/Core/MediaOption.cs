using System;
using System.Collections.Generic;

namespace Client.Core
{
    class MediaDictionary
    {
        private List<MediaOption> _options;

        public MediaDictionary()
        {
            _options = new List<MediaOption>();
        }

        public void Add(string name, string value)
        {
            MediaOption option = new MediaOption();
            option.Name = name;
            option.Value = value;

            _options.Add(option);
        }

        public void Clear()
        {
            _options.Clear();
        }

        public int Count
        {
            get
            {
                return _options.Count;
            }
        }

        public string Get(string name)
        {
            foreach (MediaOption option in _options)
            {
                if (option.Name == name)
                    return option.Value;
            }

            throw new IndexOutOfRangeException("No such option");
        }

        public string Get(string name, string def)
        {
            foreach (MediaOption option in _options)
            {
                if (option.Name == name)
                    return option.Value;
            }

            return def;
        }
    }

    struct MediaOption
    {
        public string Name;
        public string Value;
    }
}