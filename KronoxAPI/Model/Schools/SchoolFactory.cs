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
        return new School("hkr", "Kristianstad University", new string[] { "schema.hkr.se", "kronox.hkr.se" }, false);
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
        return new School("mau", "Malmö University", new string[] { "schema.mau.se" }, false);
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
        return new School("oru", "Örebro University", new string[] { "schema.oru.se", "kronox.oru.se" }, false);
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
        return new School("ltu", "Luleå University of Technology", new string[] { "schema.ltu.se" }, false);
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
        return new School("hig", "Högskolan i Gävle", new string[] { "schema.hig.se", "kronox.hig.se" }, false);
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
        return new School("sh", "Södertörns Högskola", new string[] { "kronox.sh.se" }, false);
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
        return new School("hv", "Högskolan Väst", new string[] { "schema.hv.se", "kronox.hv.se" }, false);
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
        return new School("hb", "Högskolan i Borås", new string[] { "schema.hb.se", "kronox.hb.se" }, false);
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
        return new School("mdu", "Mälardalen Högskola", new string[] { "webbschema.mdu.se", "kronox.mdu.se" }, false);
    }
}
