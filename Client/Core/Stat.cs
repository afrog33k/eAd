using System;

namespace Client.Core
{
    class Stat 
    {
        public StatType FileType;
        public String FromDate;
        public String ToDate;
        public int LayoutID;
        public int ScheduleID;
        public String MediaID;
        public String Tag;

        public override string ToString()
        {
            // Format the message into the expected XML sub nodes.
            // Just do this with a string builder rather than an XML builder.

            string theMessage = String.Format("<stat type=\"{0}\" fromdt=\"{1}\" todt=\"{2}\" layoutid=\"{3}\" scheduleid=\"{4}\" mediaid=\"{5}\"></stat>", FileType, FromDate, ToDate, LayoutID.ToString(), ScheduleID.ToString(), MediaID);

            return theMessage;
        }
    }
}