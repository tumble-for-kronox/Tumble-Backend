using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPIModels.MiscModels;

public class NotificationContent
{
    public string Topic { get; private set; }
    public string Title { get; private set; }
    public string Body { get; private set; }
    public DateTime Timestamp { get; private set; }

    public NotificationContent(string topic, string title, string body)
    {
        Topic = topic;
        Title = title;
        Body = body;
        Timestamp = DateTime.Now;
    }
}
