using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roamio.Mobile.Services;

namespace Roamio.Mobile.Models
{
    public class Trip
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Destination { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public UserPreferences UserPreferences { get; set; }
        public Dictionary<string, string> GoogleMapsData { get; set; }
        public List<string> DailyPlans { get; set; }
        public List<DayPlan> DayPlans { get; set; } = new List<DayPlan>();
    }
}
