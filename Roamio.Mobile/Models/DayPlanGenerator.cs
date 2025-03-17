using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roamio.Mobile.Models
{
    public static class DayPlanGenerator
    {
        public static void GenerateDayPlans(Trip currentTrip, List<string> restaurants, List<string> activities,
            TimeSpan startTime, TimeSpan endTime)
        {
            currentTrip.DayPlans.Clear();

            if (!DateTime.TryParse(currentTrip.StartDate, out var startDate) || !DateTime.TryParse(currentTrip.EndDate, out var endDate))
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
                    var activity = activities[d % activities.Count];
                    dayPlan.Schedule.Add(new ScheduleItem
                    {
                        Time = FormatTime(startTime),
                        Name = activity,
                        Type = "Activity"
                    });
                }

                if (restaurants.Any())
                {
                    var restaurant = restaurants[d % restaurants.Count];
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

        public static void AddNewItemsToExistingPlan(Trip currentTrip, List<string> newRestaurants, List<string> newActivities,
            TimeSpan startTime, TimeSpan endTime)
        {
            if (currentTrip.DayPlans == null || !currentTrip.DayPlans.Any()) return;

            foreach (var activity in newActivities)
            {
                PlaceNewItem(currentTrip.DayPlans, activity, "Activity", startTime, endTime);
            }
            foreach (var restaurant in newRestaurants)
            {
                PlaceNewItem(currentTrip.DayPlans, restaurant, "Restaurant", startTime, endTime);
            }
        }

        private static void PlaceNewItem(List<DayPlan> dayPlans, string itemName, string itemType,
             TimeSpan startTime, TimeSpan endTime)
        {
            foreach (var day in dayPlans)
            {
                if (day.Schedule.Count < 4)
                {
                    var timeForThisItem = CalculateTimeForNextSlot(day.Schedule, startTime, endTime);
                    day.Schedule.Add(new ScheduleItem
                    {
                        Time = FormatTime(timeForThisItem),
                        Name = itemName,
                        Type = itemType
                    });
                    return;
                }
            }
        }

        private static TimeSpan CalculateTimeForNextSlot(List<ScheduleItem> schedule, TimeSpan startTime, TimeSpan endTime)
        {            
            int itemCount = schedule.Count;
            var nextSlot = startTime.Add(TimeSpan.FromHours(2 * itemCount));         
            return nextSlot;
        }

        private static string FormatTime(TimeSpan time)
        {
            string formatted = time.ToString(@"hh\:mm");
            return (formatted == "00:00") ? "" : formatted;
        }
    }
}
