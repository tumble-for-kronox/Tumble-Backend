using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebAPIModels;

public class CourseWebModel
{
    private readonly string _id;
    private readonly string _name;
    private readonly string _color;

    public string Id => _id;

    public string Name => _name;

    public string Color => _color;

    public CourseWebModel(string id, string name, string color)
    {
        _id = id;
        _name = name;
        _color = color;
    }

    public string ToJson() => JsonSerializer.Serialize(this, new JsonSerializerOptions() { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

}
