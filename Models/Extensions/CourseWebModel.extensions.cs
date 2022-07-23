using KronoxAPI.Model.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPIModels.Extensions;

public static class CourseWebModelExtension
{
    public static CourseWebModel ToWebModel(this Course course, string color, string englishName)
    {
        return new CourseWebModel(course.Id, course.Name, color, englishName);
    }
}
