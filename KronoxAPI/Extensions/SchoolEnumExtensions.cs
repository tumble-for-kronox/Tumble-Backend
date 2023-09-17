using KronoxAPI.Model.Schools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Extensions;

public static class SchoolEnumExtensions
{
    public static string ToStringId(this SchoolEnum value)
    {
        switch (value)
        {
            case SchoolEnum.HKR:
                return "hkr";
            case SchoolEnum.MAU:
                return "mau";
            case SchoolEnum.ORU:
                return "oru";
            case SchoolEnum.LTU:
                return "ltu";
            case SchoolEnum.HIG:
                return "hig";
            case SchoolEnum.SH:
                return "sh";
            case SchoolEnum.HV:
                return "hv";
            case SchoolEnum.HB:
                return "hb";
            case SchoolEnum.MDH:
                return "mdu";
            default:
                return "";
        }
    }
}
