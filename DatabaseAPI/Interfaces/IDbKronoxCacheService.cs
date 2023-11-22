using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPIModels.MiscModels;

namespace DatabaseAPI.Interfaces;

public interface IDbKronoxCacheService
{
    public Task<KronoxCache?> GetKronoxCacheAsync(string id);

    public Task UpsertKronoxCacheAsync(KronoxCache kronoxCache);
}
