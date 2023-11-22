using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPIModels.ResponseModels;

namespace DatabaseAPI.Interfaces;

public interface IDbSchedulesService
{
    public Task<ScheduleWebModel?> GetScheduleAsync(string id);

    public Task UpsertScheduleAsync(ScheduleWebModel schedule);
}
