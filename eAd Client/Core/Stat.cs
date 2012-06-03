namespace ClientApp.Core
{
    using System;

    public class Stat
    {
        public StatType FileType;
        public string FromDate;
        public int LayoutID;
        public string MediaID;
        public int ScheduleID;
        public string Tag;
        public string ToDate;

        public override string ToString()
        {
            return string.Format("<stat type=\"{0}\" fromdt=\"{1}\" todt=\"{2}\" layoutid=\"{3}\" scheduleid=\"{4}\" mediaid=\"{5}\"></stat>", new object[] { this.FileType, this.FromDate, this.ToDate, this.LayoutID.ToString(), this.ScheduleID.ToString(), this.MediaID });
        }
    }
}

