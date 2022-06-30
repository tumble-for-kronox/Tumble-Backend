using KronoxAPI.Controller;
using KronoxAPI.Model.Scheduling;
using System.Xml.Linq;
using System.Xml;
using KronoxAPI.Utilities;
using KronoxAPI.Parser;
using KronoxAPI.Model.Response;
using KronoxAPI.Model.Users;
using KronoxAPI.Model.Schools;

// See https://aka.ms/new-console-template for more information

School chosenSchool = SchoolFactory.Hkr();
User user = chosenSchool.Login("lasse_koordt_rosenkrans.poulsen0003@stud.hkr.s", "bAFKfmX#ME%^R7u");

Console.WriteLine(user.Name);
Console.WriteLine(user.Username);
Console.WriteLine(user.SessionToken);