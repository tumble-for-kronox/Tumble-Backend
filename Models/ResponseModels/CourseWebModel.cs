using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebAPIModels.ResponseModels;

public class CourseWebModel
{
    private readonly string _id;
    private readonly string _swedishName;
    private readonly string _englishName;

    public string Id => _id;

    public string SwedishName => _swedishName;

    public string EnglishName => _englishName;

    public CourseWebModel(string id, string swedishName, string englishName)
    {
        _id = id;
        _swedishName = swedishName;
        _englishName = englishName;
    }

    public string ToJson() => JsonSerializer.Serialize(this, new JsonSerializerOptions() { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

}
