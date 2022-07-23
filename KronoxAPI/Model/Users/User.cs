using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using KronoxAPI.Controller;
using KronoxAPI.Model.Schools;
using KronoxAPI.Parser;
using KronoxAPI.Exceptions;

namespace KronoxAPI.Model.Users;

/// <summary>
/// Model for fetching and managing user data in Kronox's database. 
/// </summary>
public class User
{
    private readonly string _name;
    private readonly string _username;
    private string _sessionToken;

    public User(string name, string username, string sessionToken)
    {
        _name = name;
        _username = username;
        _sessionToken = sessionToken;
    }

    public string Name => _name;
    public string Username => _username;
    public string SessionToken { get => _sessionToken; set => _sessionToken = value; }

    /// <summary>
    /// For use as default or in case a user is not found.
    /// </summary>
    /// <returns><see cref="User"/> with all values set as "N/A".</returns>
    public static User NotAvailable => new("N/A", "N/A", "N/A");
}
