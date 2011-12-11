namespace eAd.DataViewModels
{
    public class MessageViewModel
    {
        public long ID
        {
            get;
            set;
        }

        public string Command { get; set; }

        public string Type
        {
            get;
            set;
        }

        public string Text
        {
            get;
            set;
        }

        public long StationID
        {
            get; set; }

        public long UserID { get; set; }

        public bool Sent { get; set; }
    }
}