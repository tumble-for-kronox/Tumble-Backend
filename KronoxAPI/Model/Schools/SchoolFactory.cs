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
    /// url          : "schema.hkr.se"
    /// loginRequired: false
    /// </code>
    /// </summary>
    /// <returns></returns>
    public static School Hkr()
    {
        return new School("hkr", "Kristianstad University", "schema.hkr.se", false);
    }

    /// <summary>
    /// Returns a <see cref="School"/> instance:
    /// <code>
    /// schoolId     : "mau"
    /// schoolName   : "Malmö University"
    /// url          : "schema.mau.se"
    /// loginRequired: false
    /// </code>
    /// </summary>
    /// <returns></returns>
    public static School Mau()
    {
        return new School("mau", "Malmö University", "schema.mau.se", false);
    }

    /// <summary>
    /// Returns a <see cref="School"/> instance:
    /// <code>
    /// schoolId     : "oru"
    /// schoolName   : "Örebro University"
    /// url          : "schema.oru.se"
    /// loginRequired: false
    /// </code>
    /// </summary>
    /// <returns></returns>
    public static School Oru()
    {
        return new School("oru", "Örebro University", "schema.oru.se", false);
    }

    /// <summary>
    /// Returns a <see cref="School"/> instance:
    /// <code>
    /// schoolId     : "ltu"
    /// schoolName   : "Luleå University of Technology"
    /// url          : "schema.ltu.se"
    /// loginRequired: true
    /// </code>
    /// </summary>
    /// <returns></returns>
    public static School Ltu()
    {
        return new School("ltu", "Luleå University of Technology", "schema.ltu.se", true);
    }

    /// <summary>
    /// Returns a <see cref="School"/> instance:
    /// <code>
    /// schoolId     : "hig"
    /// schoolName   : "Högskolan i Gävle"
    /// url          : "schema.hig.se"
    /// loginRequired: false
    /// </code>
    /// </summary>
    /// <returns></returns>
    public static School Hig()
    {
        return new School("hig", "Högskolan i Gävle", "schema.hig.se", false);
    }

    /// <summary>
    /// Returns a <see cref="School"/> instance:
    /// <code>
    /// schoolId     : "sh"
    /// schoolName   : "Södertörns Högskola"
    /// url          : "kronox.sh.se"
    /// loginRequired: false
    /// </code>
    /// </summary>
    /// <returns></returns>
    public static School Sh()
    {
        return new School("sh", "Södertörns Högskola", "kronox.sh.se", false);
    }

    /// <summary>
    /// Returns a <see cref="School"/> instance:
    /// <code>
    /// schoolId     : "hv"
    /// schoolName   : "Högskolan Väst"
    /// url          : "schema.hv.se"
    /// loginRequired: false
    /// </code>
    /// </summary>
    /// <returns></returns>
    public static School Hv()
    {
        return new School("hv", "Högskolan Väst", "schema.hv.se", false);
    }

    /// <summary>
    /// Returns a <see cref="School"/> instance:
    /// <code>
    /// schoolId     : "hb"
    /// schoolName   : "Högskolan i Borås"
    /// url          : "schema.hb.se"
    /// loginRequired: false
    /// </code>
    /// </summary>
    /// <returns></returns>
    public static School Hb()
    {
        return new School("hb", "Högskolan i Borås", "schema.hb.se", false);
    }

    /// <summary>
    /// Returns a <see cref="School"/> instance:
    /// <code>
    /// schoolId     : "mdh"
    /// schoolName   : "Mälardalen Högskola"
    /// url          : "schema.mdh.se"
    /// loginRequired: true
    /// </code>
    /// </summary>
    /// <returns></returns>
    public static School Mdh()
    {
        return new School("mdh", "Mälardalen Högskola", "schema.mdh.se", true);
    }
}
