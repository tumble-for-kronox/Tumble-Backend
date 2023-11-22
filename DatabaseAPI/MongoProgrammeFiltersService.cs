using DatabaseAPI.Interfaces;
using KronoxAPI.Extensions;
using KronoxAPI.Model.Schools;
using MongoDB.Driver;
using WebAPIModels.MiscModels;

namespace DatabaseAPI;

public class MongoProgrammeFiltersService : IDbProgrammeFiltersService
{
    private readonly IMongoCollection<SchoolProgrammeFilter> _programmeFiltersCollection;

    public MongoProgrammeFiltersService(IDbSettings settings)
    {
        MongoClient client = new(settings.ConnectionString);
        IMongoDatabase database = client.GetDatabase(settings.DatabaseName);
        _programmeFiltersCollection = database.GetCollection<SchoolProgrammeFilter>("programme_filters");
    }

    public async Task<HashSet<string>> GetProgrammeFiltersAsync(School school)
    {
        List<SchoolProgrammeFilter> cursor = await _programmeFiltersCollection.Find(Builders<SchoolProgrammeFilter>.Filter.Eq("_id", school.Id.ToStringId())).ToListAsync();

        if (cursor.Any())
            return cursor.First().Filter.ToHashSet();

        return new();
    }
}
