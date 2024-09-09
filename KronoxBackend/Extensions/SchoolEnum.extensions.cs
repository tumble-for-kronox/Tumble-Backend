using KronoxAPI.Model.Schools;

namespace KronoxBackend.Extensions;

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
            case SchoolEnum.ORU:
                return SchoolFactory.Oru();
            case SchoolEnum.LTU:
                return SchoolFactory.Ltu();
            case SchoolEnum.HIG:
                return SchoolFactory.Hig();
            case SchoolEnum.SH:
                return SchoolFactory.Sh();
            case SchoolEnum.HV:
                return SchoolFactory.Hv();
            case SchoolEnum.HB:
                return SchoolFactory.Hb();
            case SchoolEnum.MDH:
                return SchoolFactory.Mdh();
            default:
                return null;
        }
    }
}
