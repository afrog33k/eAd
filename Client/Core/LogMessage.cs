
using System;

namespace Client.Core
{
class LogMessage
{
    String _method;
    String _message;
    int _scheduleId;
    int _layoutId;
    int _mediaId;

    public LogMessage(String method, String message)
    {
        _method = method;
        _message = message;
    }

    public LogMessage(String method, String message, int scheduleId, int layoutId)
    {
        _method = method;
        _message = message;
        _scheduleId = scheduleId;
        _layoutId = layoutId;
    }

    public LogMessage(String method, String message, int scheduleId, int layoutId, int mediaId)
    {
        _method = method;
        _message = message;
        _scheduleId = scheduleId;
        _layoutId = layoutId;
        _mediaId = mediaId;
    }

    public override string ToString()
    {
        // Format the message into the expected XML sub nodes.
        // Just do this with a string builder rather than an XML builder.
        String theMessage;

        theMessage = String.Format("<message>{0}</message>", _message);
        theMessage += String.Format("<method>{0}</method>", _method);

        if (_scheduleId != 0) theMessage += String.Format("<scheduleid>{0}</scheduleid>", _scheduleId.ToString());
        if (_layoutId != 0) theMessage += String.Format("<layoutid>{0}</layoutid>", _scheduleId.ToString());
        if (_mediaId != 0) theMessage += String.Format("<mediaid>{0}</mediaid>", _scheduleId.ToString());

        return theMessage;
    }
}
}
