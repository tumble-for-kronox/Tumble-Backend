using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPIModels.MiscModels;

namespace WebAPIModels.ResponseModels;

public class KronoxUserWebModel
{
    private readonly string _name;
    private readonly string _username;
    private readonly string _refreshToken;
    private readonly SessionDetails _sessionDetails;


    public string Name => _name;

    public string Username => _username;

    public string RefreshToken => _refreshToken;

    [Obsolete("This token is only used temporarily while passing to web api. Do not use!")]
    public SessionDetails SessionDetails => _sessionDetails;

    public KronoxUserWebModel(string name, string username, string refreshToken, SessionDetails sessionDetails)
    {
        _name = name;
        _username = username;
        this._refreshToken = refreshToken;
        this._sessionDetails = sessionDetails;
    }

}
