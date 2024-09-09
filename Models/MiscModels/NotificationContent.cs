using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPIModels.MiscModels;

[BsonIgnoreExtraElements]
public class NotificationContent
{
    public string Topic { get; set; }

    public string Title { get; set; }

    public string Body { get; set; }

    public string? LongBody { get; set; }

    [BsonElement("timestamp")]
    public DateTime Timestamp { get; } = DateTime.Now;
}
