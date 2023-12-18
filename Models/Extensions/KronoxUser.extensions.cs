using KronoxAPI.Model.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPIModels.MiscModels;
using WebAPIModels.ResponseModels;

namespace WebAPIModels.Extensions;

public static class KronoxUserExtensions
{
    public static KronoxUserWebModel ToWebModel(this User user, string refreshToken, SessionDetails sessionDetails)
    {
        return new(user.Name, user.Username, refreshToken, sessionDetails);
    }
}
