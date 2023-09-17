using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Extensions;

public static class CookieContainerExtensions
{
    public static Cookie? GetCookie(this CookieContainer cookies, Uri uri, string name)
    {
        return cookies.GetCookies(uri).FirstOrDefault(c => c.Name == name);
    }
}
