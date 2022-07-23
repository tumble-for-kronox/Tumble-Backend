using KronoxAPI.Model.Schools;
using TumbleBackend.Utilities;

namespace TumbleBackend.Extensions;

public static class SchoolEnumExtensions
{
    public static School? GetSchool(this SchoolEnum schoolId)
    {
        switch (schoolId)
        {
            case SchoolEnum.HKR:
                return SchoolFactory.Hkr();
            case SchoolEnum.MAU:
                return SchoolFactory.Mau();
            default:
                return null;
        }
    }
}
