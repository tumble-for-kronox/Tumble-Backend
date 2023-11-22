using DatabaseAPI.Interfaces;
using KronoxAPI.Model.Scheduling;
using MongoDB.Driver;
using WebAPIModels.MiscModels;
using WebAPIModels.ResponseModels;

namespace DatabaseAPI;

public class MongoKronoxCacheService : IDbKronoxCacheService
{
    private readonly IMongoCollection<KronoxCache> _kronoxCacheCollection;

    public MongoKronoxCacheService(IDbSettings settings)
    {
        MongoClient client = new(settings.ConnectionString);
        IMongoDatabase database = client.GetDatabase(settings.DatabaseName);
        _kronoxCacheCollection = database.GetCollection<KronoxCache>("kronox_cache");
    }

    public async Task<KronoxCache?> GetKronoxCacheAsync(string id)
    {
        List<KronoxCache> cacheItems = await _kronoxCacheCollection.Find(Builders<KronoxCache>.Filter.Eq("_id", id)).Limit(1).ToListAsync();

        if (cacheItems.Count == 0)
        {
            return null;
        }

        return cacheItems.First();
    }

    public async Task UpsertKronoxCacheAsync(KronoxCache kronoxCache)
    {
        var filter = Builders<KronoxCache>.Filter.Eq("_id", kronoxCache.Id);

        var options = new ReplaceOptions { IsUpsert = true };
        await _kronoxCacheCollection.ReplaceOneAsync(filter, kronoxCache, options);
    }
}
