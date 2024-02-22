using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPIModels.MiscModels;

namespace WebAPIModels.ResponseModels;

public class KronoxUserWebModel
{
    public string Name { get; }

    public string Username { get; }

    public string RefreshToken { get; }
    
    public string SessionToken { get; }

    public SessionDetails SessionDetails { get; }

    public KronoxUserWebModel(string name, string username, string refreshToken, string sessionToken, SessionDetails sessionDetails)
    {
        Name = name;
        Username = username;
        RefreshToken = refreshToken;
        SessionToken = sessionToken;
        SessionDetails = sessionDetails;
    }

}
