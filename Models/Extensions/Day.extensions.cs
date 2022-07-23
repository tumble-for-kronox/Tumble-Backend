using KronoxAPI.Model.Scheduling;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPIModels.ResponseModels;

namespace WebAPIModels.Extensions;

public static class DayExtensions
{
    public static DayWebModel ToWebModel(this Day day)
    {
        return new DayWebModel(day.Name, day.Date.ToString("d/M", CultureInfo.InvariantCulture), day.Date.Year, day.Date.Month, day.Date.Day, (int)day.Date.DayOfWeek, day.WeekNumber, day.Events);
    }
}
