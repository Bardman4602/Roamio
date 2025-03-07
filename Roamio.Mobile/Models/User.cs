using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roamio.Mobile.Services;

namespace Roamio.Mobile.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string HashedPassword { get; set; }
        public Dictionary<string, string> Preferences { get; set; }
        public string CurrentTrip { get; set; }
        public List<string> TripHistory { get; set; }
        public string DailyPlans { get; set; }
    }
}
