using KronoxAPI.Model.Schools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAPI.Interfaces;

public interface IDbProgrammeFiltersService
{
    public Task<HashSet<string>> GetProgrammeFiltersAsync(School school);
}
