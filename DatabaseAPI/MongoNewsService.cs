using DatabaseAPI.Interfaces;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using WebAPIModels.MiscModels;

namespace DatabaseAPI;

public class MongoNewsService : IDbNewsService
{
    private readonly IMongoCollection<NotificationContent> _newsCollection;

    public MongoNewsService(IDbSettings settings)
    {
        MongoClient client = new(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);
        _newsCollection = database.GetCollection<NotificationContent>("news_history");
    }

    public async Task<List<NotificationContent>> GetNewsHistoryAsync()
    {
        var cursor = await _newsCollection.AsQueryable().OrderByDescending(c => c.Timestamp).Take(10).ToListAsync();

        return cursor.Any() ? cursor : new List<NotificationContent>();
    }

    public async Task SaveNewsItemAsync(NotificationContent item)
    {
        await _newsCollection.InsertOneAsync(item);
    }
}
