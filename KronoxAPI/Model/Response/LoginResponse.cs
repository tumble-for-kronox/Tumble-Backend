using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Model.Response;

public class LoginResponse
{
    public readonly string sessionToken;
    public readonly string htmlResult;

    public LoginResponse(string sessionToken, string htmlResult)
    {
        this.sessionToken = sessionToken;
        this.htmlResult = htmlResult;
    }
}
