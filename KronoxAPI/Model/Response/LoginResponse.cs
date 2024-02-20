using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Model.Response;

public class LoginResponse
{
    public readonly string SessionToken;
    public readonly string HtmlResult;

    public LoginResponse(string sessionToken, string htmlResult)
    {
        SessionToken = sessionToken;
        HtmlResult = htmlResult;
    }
}
