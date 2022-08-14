using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPIModels.MiscModels;

public class SchoolProgrammeFilter
{
    public SchoolProgrammeFilter(string id, List<string> filter)
    {
        Id = id;
        Filter = filter;
    }

    public string Id { get; set; }

    public List<string> Filter { get; set; }


}
