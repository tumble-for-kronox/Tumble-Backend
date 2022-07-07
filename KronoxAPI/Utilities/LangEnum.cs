using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Utilities;

public class LangEnum
{
    private LangEnum(string value)
    {
        Value = value;
    }

    public string Value { get; private set; }

    public static LangEnum En => new("EN");
    public static LangEnum Sv => new("SV");
}
