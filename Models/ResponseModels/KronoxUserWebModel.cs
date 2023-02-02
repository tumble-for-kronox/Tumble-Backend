using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPIModels.ResponseModels;

public class KronoxUserWebModel
{
    private readonly string _name;
    private readonly string _username;
    private readonly string _refreshToken;
    private readonly string _sessionToken;


    public string Name => _name;

    public string Username => _username;

    public string RefreshToken => _refreshToken;

    [Obsolete("This token is only used temporarily while passing to web api. Do not use!")]
    public string SessionToken => _sessionToken;

    public KronoxUserWebModel(string name, string username, string sessionToken, string refreshToken)
    {
        _name = name;
        _username = username;
        this._refreshToken = refreshToken;
        this._sessionToken = sessionToken;
    }

}
