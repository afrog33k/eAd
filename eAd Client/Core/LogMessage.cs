namespace ClientApp.Core
{
    using System;

    internal class LogMessage
    {
        private int _layoutId;
        private int _mediaId;
        private string _message;
        private string _method;
        private int _scheduleId;

        public LogMessage(string method, string message)
        {
            this._method = method;
            this._message = message;
        }

        public LogMessage(string method, string message, int scheduleId, int layoutId)
        {
            this._method = method;
            this._message = message;
            this._scheduleId = scheduleId;
            this._layoutId = layoutId;
        }

        public LogMessage(string method, string message, int scheduleId, int layoutId, int mediaId)
        {
            this._method = method;
            this._message = message;
            this._scheduleId = scheduleId;
            this._layoutId = layoutId;
            this._mediaId = mediaId;
        }

        public override string ToString()
        {
            string str = string.Format("<message>{0}</message>", this._message) + string.Format("<method>{0}</method>", this._method);
            if (this._scheduleId != 0)
            {
                str = str + string.Format("<scheduleid>{0}</scheduleid>", this._scheduleId.ToString());
            }
            if (this._layoutId != 0)
            {
                str = str + string.Format("<layoutid>{0}</layoutid>", this._scheduleId.ToString());
            }
            if (this._mediaId != 0)
            {
                str = str + string.Format("<mediaid>{0}</mediaid>", this._scheduleId.ToString());
            }
            return str;
        }
    }
}

