using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPIModels.MiscModels;

public class NotificationContent
{
    public string Topic { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public readonly DateTime timestamp = DateTime.Now;

}
