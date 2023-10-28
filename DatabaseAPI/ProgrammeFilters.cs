using KronoxAPI.Extensions;
using KronoxAPI.Model.Schools;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPIModels.MiscModels;

namespace DatabaseAPI;

public static class ProgrammeFilters
{
    public static async Task<HashSet<string>> GetProgrammeFilter(School school)
    {
        if (Connector.ProgrammeFilters == null) throw new Exceptions.DatabaseUninitializedException("The database/collection is not initialized. Run .Init() on the Connector before attempting to access the database.");

        List<SchoolProgrammeFilter> cursor = await Connector.ProgrammeFilters.Find(Builders<SchoolProgrammeFilter>.Filter.Eq("_id", school.Id.ToStringId())).ToListAsync();

        if (cursor.Any())
            return cursor.First().Filter.ToHashSet();

        return new();
    }
}
