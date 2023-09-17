using KronoxAPI.Model.Schools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPIModels.RequestModels;

public class MultiSchoolSchedules
{
    public SchoolEnum SchoolId { get; set; }
    public string[] ScheduleIds { get; set; }

    public MultiSchoolSchedules(SchoolEnum schoolId, string[] scheduleIds)
    {
        SchoolId = schoolId;
        ScheduleIds = scheduleIds;
    }

    public MultiSchoolSchedules() { }
}
