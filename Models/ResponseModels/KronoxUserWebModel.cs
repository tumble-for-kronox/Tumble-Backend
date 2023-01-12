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
    private readonly string refreshToken;


    public string Name => _name;

    public string Username => _username;

    public string RefreshToken => refreshToken;

    public KronoxUserWebModel(string name, string username, string refreshToken)
    {
        _name = name;
        _username = username;
        this.refreshToken = refreshToken;
    }

}
