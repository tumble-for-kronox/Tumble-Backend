using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Model.Schools;

/// <summary>
/// Factory for constructing and returning <see cref="School"/> instances that are pre-filled
/// with data for the schools in Kronox's database.
/// </summary>
public class SchoolFactory
{
    /// <summary>
    /// Returns a <see cref="School"/> instance:
    /// <code>
    /// schoolId     : "hkr"
    /// schoolName   : "Kristianstad University"
    /// urls          : ["schema.hkr.se", "kronox.hkr.se"]
    /// loginRequired: false
    /// </code>
    /// </summary>
    /// <returns></returns>
    public static School Hkr()
    {
        return new School(SchoolEnum.HKR, "Kristianstad University", new string[] { "https://schema.hkr.se/", "https://kronox.hkr.se/" }, false);
    }

    /// <summary>
    /// Returns a <see cref="School"/> instance:
    /// <code>
    /// schoolId     : "mau"
    /// schoolName   : "Malmö University"
    /// urls          : ["schema.mau.se"]
    /// loginRequired: false
    /// </code>
    /// </summary>
    /// <returns></returns>
    public static School Mau()
    {
        return new School(SchoolEnum.MAU, "Malmö University", new string[] { "https://schema.mau.se/" }, false);
    }

    /// <summary>
    /// Returns a <see cref="School"/> instance:
    /// <code>
    /// schoolId     : "oru"
    /// schoolName   : "Örebro University"
    /// urls          : ["schema.oru.se", "kronox.oru.se"]
    /// loginRequired: false
    /// </code>
    /// </summary>
    /// <returns></returns>
    public static School Oru()
    {
        return new School(SchoolEnum.ORU, "Örebro University", new string[] { "https://schema.oru.se/", "https://kronox.oru.se/" }, false);
    }

    /// <summary>
    /// Returns a <see cref="School"/> instance:
    /// <code>
    /// schoolId     : "ltu"
    /// schoolName   : "Luleå University of Technology"
    /// urls          : ["schema.ltu.se"]
    /// loginRequired: false
    /// </code>
    /// </summary>
    /// <returns></returns>
    public static School Ltu()
    {
        return new School(SchoolEnum.LTU, "Luleå University of Technology", new string[] { "https://schema.ltu.se/" }, false);
    }

    /// <summary>
    /// Returns a <see cref="School"/> instance:
    /// <code>
    /// schoolId     : "hig"
    /// schoolName   : "Högskolan i Gävle"
    /// urls          : ["schema.hig.se", "kronox.hig.se"]
    /// loginRequired: false
    /// </code>
    /// </summary>
    /// <returns></returns>
    public static School Hig()
    {
        return new School(SchoolEnum.HIG, "Högskolan i Gävle", new string[] { "https://schema.hig.se/", "https://kronox.hig.se/" }, false);
    }

    /// <summary>
    /// Returns a <see cref="School"/> instance:
    /// <code>
    /// schoolId     : "sh"
    /// schoolName   : "Södertörns Högskola"
    /// urls          : ["kronox.sh.se"]
    /// loginRequired: false
    /// </code>
    /// </summary>
    /// <returns></returns>
    public static School Sh()
    {
        return new School(SchoolEnum.SH, "Södertörns Högskola", new string[] { "https://kronox.sh.se/" }, false);
    }

    /// <summary>
    /// Returns a <see cref="School"/> instance:
    /// <code>
    /// schoolId     : "hv"
    /// schoolName   : "Högskolan Väst"
    /// urls          : ["schema.hv.se", "kronox.hv.se"]
    /// loginRequired: false
    /// </code>
    /// </summary>
    /// <returns></returns>
    public static School Hv()
    {
        return new School(SchoolEnum.HV, "Högskolan Väst", new string[] { "https://schema.hv.se/", "https://kronox.hv.se/" }, false);
    }

    /// <summary>
    /// Returns a <see cref="School"/> instance:
    /// <code>
    /// schoolId     : "hb"
    /// schoolName   : "Högskolan i Borås"
    /// urls          : ["schema.hb.se", "kronox.hb.se"]
    /// loginRequired: false
    /// </code>
    /// </summary>
    /// <returns></returns>
    public static School Hb()
    {
        return new School(SchoolEnum.HB, "Högskolan i Borås", new string[] { "https://schema.hb.se/", "https://kronox.hb.se/" }, false);
    }

    /// <summary>
    /// Returns a <see cref="School"/> instance:
    /// <code>
    /// schoolId     : "mdh"
    /// schoolName   : "Mälardalen Högskola"
    /// urls          : ["schema.mdu.se"]
    /// loginRequired: false
    /// </code>
    /// </summary>
    /// <returns></returns>
    public static School Mdh()
    {
        return new School(SchoolEnum.MDH, "Mälardalen Högskola", new string[] { "https://webbschema.mdu.se/", "https://kronox.mdu.se/" }, false);
    }
}
