using KronoxAPI.Model.Schools;

namespace WebAPIModels.MiscModels;

public class KronoxCache
{
    public KronoxCache(string id, string url, DateTime timeStamp, SchoolEnum school)
    {
        Id = id;
        Url = url;
        Timestamp = timeStamp;
        School = school;
    }

    public string Id { get; set; }

    public SchoolEnum School { get; set; }

    public string Url { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.Now;
}
