using KronoxAPI.Controller;
using KronoxAPI.Model.Scheduling;
using DatabaseAPI;
using WebAPIModels;
using WebAPIModels.Extensions;
using KronoxAPI.Model.Schools;

// See https://aka.ms/new-console-template for more information

Connector.Init("");

//School school = SchoolFactory.Hkr();
//Schedule schedule = school.FetchSchedule("p.TBSE2+2020+36+100+NML+en");
//ScheduleWebModel scheduleWeb = schedule.ToWebModel();

Console.WriteLine(SchedulesCollection.GetSchedule("p.TBSE2+2020+36+100+NML+en").ToJson());
