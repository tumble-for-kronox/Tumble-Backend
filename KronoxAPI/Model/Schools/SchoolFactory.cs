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
}
