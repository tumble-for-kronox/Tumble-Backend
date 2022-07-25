using KronoxAPI.Model.Scheduling;

namespace TumbleBackend.Extensions
{
    public static class ScheduleExtensions
    {
        public static Dictionary<string, Course> Courses(this Schedule schedule)
        {
            Dictionary<string, Course> courses = new();

            foreach (Day day in schedule.Days)
            {
                foreach (Event e in day.Events)
                {
                    if (!courses.ContainsKey(e.Course.Id))
                    {
                        courses.Add(e.Course.Id, e.Course);
                    }
                }
            }

            return courses;
        }
    }
}
