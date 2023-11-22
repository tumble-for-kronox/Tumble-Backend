using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPIModels.MiscModels;

namespace DatabaseAPI.Interfaces;

public interface IDbNewsService
{
    public Task<List<NotificationContent>> GetNewsHistoryAsync();

    public Task SaveNewsItemAsync(NotificationContent item);
}
