using KronoxAPI.Controller;
using KronoxAPI.Model.Scheduling;
using System.Xml.Linq;
using System.Xml;
using KronoxAPI.Utilities;
using KronoxAPI.Parser;
using KronoxAPI.Model.Response;
using KronoxAPI.Model.Users;
using KronoxAPI.Model.Schools;
using HtmlAgilityPack;

// See https://aka.ms/new-console-template for more information

School hkr = SchoolFactory.Hkr();
User timmy = hkr.Login("usernameHere", "passwordHere");

Dictionary<string, List<UserEvent>> userEvents = timmy.GetUserEvents(hkr);

Console.WriteLine("REGISTERED EVENTS ------------------------");
userEvents["registered"].ForEach(el =>
{
    AvailableUserEvent userEvent = (AvailableUserEvent)el;

    Console.WriteLine(userEvent.Id);
    Console.WriteLine(userEvent.Title);
    Console.WriteLine(userEvent.LastSignupDate.ToString());
    Console.WriteLine(userEvent.EventStart.ToString());
    Console.WriteLine(userEvent.EventEnd.ToString());
    Console.WriteLine(userEvent.IsRegistered);
    Console.WriteLine(userEvent.SupportAvailable);
    Console.WriteLine(userEvent.ParticipatorId);
    Console.WriteLine(userEvent.SupportId);
    Console.WriteLine(userEvent.Type);
    Console.WriteLine(userEvent.AnonymousCode);
    Console.WriteLine("\n");

});

Console.WriteLine("UNREGISTERED EVENTS ------------------------");
userEvents["unregistered"].ForEach(el =>
{
    AvailableUserEvent userEvent = (AvailableUserEvent)el;

    Console.WriteLine(userEvent.Id);
    Console.WriteLine(userEvent.Title);
    Console.WriteLine(userEvent.LastSignupDate.ToString());
    Console.WriteLine(userEvent.EventStart.ToString());
    Console.WriteLine(userEvent.EventEnd.ToString());
    Console.WriteLine(userEvent.IsRegistered);
    Console.WriteLine(userEvent.SupportAvailable);
    Console.WriteLine(userEvent.ParticipatorId);
    Console.WriteLine(userEvent.SupportId);
    Console.WriteLine(userEvent.Type);
    Console.WriteLine(userEvent.AnonymousCode);
    Console.WriteLine("\n");
});

Console.WriteLine("UPCOMING EVENTS ------------------------");
userEvents["upcoming"].ForEach(el =>
{
    UpcomingUserEvent userEvent = (UpcomingUserEvent)el;

    Console.WriteLine(userEvent.Title);
    Console.WriteLine(userEvent.EventStart.ToString());
    Console.WriteLine(userEvent.EventEnd.ToString());
    Console.WriteLine(userEvent.Type);
    Console.WriteLine("\n");
});

