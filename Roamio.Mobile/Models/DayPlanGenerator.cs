using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roamio.Mobile.Models
{
    public static class DayPlanGenerator
    {
        public static void GenerateDayPlans(Trip currentTrip, List<string> restaurants, List<string> activities, TimeSpan startTime, TimeSpan endTime)
        {
            currentTrip.DayPlans.Clear();

            if (!DateTime.TryParse(currentTrip.StartDate, out var startDate) ||
                !DateTime.TryParse(currentTrip.EndDate, out var endDate))
            {
                return;
            }

            int totalDays = (endDate - startDate).Days + 1;            
            for (int d = 0; d < totalDays; d++)
            {
                var dayPlan = new DayPlan
                {
                    DayNumber = d + 1,
                    Date = startDate.AddDays(d),
                    Schedule = new List<ScheduleItem>()
                };
                                
                if (activities.Any())
                {
                    string activity = activities[d % activities.Count];
                    dayPlan.Schedule.Add(new ScheduleItem
                    {
                        Time = FormatTime(startTime),
                        Name = activity,
                        Type = "Activity"
                    });
                }
                                
                if (restaurants.Any())
                {
                    string restaurant = restaurants[d % restaurants.Count];                    
                    dayPlan.Schedule.Add(new ScheduleItem
                    {
                        Time = FormatTime(startTime.Add(TimeSpan.FromHours(2))),
                        Name = restaurant,
                        Type = "Restaurant"
                    });
                }

                currentTrip.DayPlans.Add(dayPlan);
            }
        }

        private static string FormatTime(TimeSpan time)
        {
            string formatted = time.ToString("hh\\:mm");
            return (formatted == "00:00") ? "" : formatted;
        }
    }
}
