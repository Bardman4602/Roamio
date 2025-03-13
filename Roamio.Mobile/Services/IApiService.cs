using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roamio.Mobile.Models;

namespace Roamio.Mobile.Services
{
    public interface IApiService
    {
        Task<User> GetUserAsync(string id);
        Task<User> CreateUserAsync(User user);
        Task<Trip> CreateTripAsync(Trip trip);
        Task<Trip> GetTripAsync(string id, string userId);
        Task<DayPlan> GetDayPlanAsync(string tripId, string date);
        Task<GoogleMapsData> GetGoogleMapsDataAsync(string userId, string location);
        Task<UserPreferences> GetUserPreferencesAsync(string id, string userId);
    }
}
