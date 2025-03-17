using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roamio.Mobile.Services;

namespace Roamio.Mobile.Models
{
    public class DayPlan
    {
        //public string TripId { get; set; }
        //public string Date { get; set; }
        //public string Id { get; set; }
        //public Dictionary<string, object> Schedule { get; set; }
        //public List<string> StartTimes { get; set; }
        //public List<string> EndTimes { get; set; }
        //public List<string> Suggestions { get; set; }
        //public Dictionary<string, object> GoogleMapsData { get; set; }
        //public Dictionary<string, object> UserPreferences { get; set; }
        //public Dictionary<string, object> UserSelections { get; set; }

        public int DayNumber { get; set; }
        public DateTime Date { get; set; }
        public List<ScheduleItem> Schedule { get; set; } = new List<ScheduleItem>();
        public string Summary { get; set; } //Maybe change to a list of schedule items for the day?
    }

    public class ScheduleItem
    {
        public string Time { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string LocationQuery { get; set; }
    }
}
