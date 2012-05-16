using System;

namespace ClientApp.Core
{
[Serializable]
struct TraceMessage
{
    public String Message;
    public String DateTime;
    public String Category;
}
}