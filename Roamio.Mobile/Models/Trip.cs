using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roamio.Mobile.Services;
using CommunityToolkit.Mvvm.Messaging.Messages;
using SQLite;

namespace Roamio.Mobile.Models
{
    public class Trip
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Destination { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        [Ignore] public UserPreferences UserPreferences { get; set; }
        public string UserPreferencesJson { get; set; }

        [Ignore] public Dictionary<string, string> GoogleMapsData { get; set; }
        public string GoogleMapsDataJson { get; set; }

        [Ignore] public List<string> DailyPlans { get; set; }
        public string DailyPlansJson { get; set; }

        [Ignore] public List<string> RestaurantSelections { get; set; } = new List<string>();
        public string RestaurantSelectionsJson { get; set; }

        [Ignore] public List<string> ActivitySelections { get; set; } = new List<string>();
        public string ActivitySelectionsJson { get; set; }

        [Ignore] public List<DayPlan> DayPlans { get; set; } = new List<DayPlan>();
        public string DayPlansJson { get; set; }

        public Trip()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.NewGuid().ToString();
            }
        }
    }

    public class TripUpdatedMessage : ValueChangedMessage<Trip>
    {
        public TripUpdatedMessage(Trip trip) : base(trip)
        {
        }
    }
}
