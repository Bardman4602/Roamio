using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Roamio.Mobile.Models;

namespace Roamio.Mobile.Services
{
    class ApiService : IApiService
    {
        private readonly HttpClient _client;
        
        public ApiService(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient("APIClient");
        }

        public async Task<User> GetUserAsync(string id)
        {
            var response = await _client.GetAsync($"/User/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<User>();
            }
            return null;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            var response = await _client.PostAsJsonAsync("/User", user);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Error creating user: {response.StatusCode} - {errorContent}");
                return null;
            }

            try
            {
                var json = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine("Raw JSON from server: " + json);

                // Manually deserialize so you can catch errors
                var createdUser = System.Text.Json.JsonSerializer.Deserialize<User>(json);
                return createdUser;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Deserialization error: {ex.Message}");
                return null;
            }
        }

        public async Task<Trip> GetTripAsync(string id, string userId)
        {
            var response = await _client.GetAsync($"/Trip/{id}/{userId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Trip>();
            }
            return null;
        }

        public async Task<DayPlan> GetDayPlanAsync(string tripId, string date)
        {
            var response = await _client.GetAsync($"/DayPlan/{tripId}/{date}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<DayPlan>();
            }
            return null;
        }

        public async Task<GoogleMapsData> GetGoogleMapsDataAsync(string userId, string location)
        {
            var response = await _client.GetAsync($"/GoogleMapsData/{userId}/{location}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<GoogleMapsData>();
            }
            return null;
        }

        public async Task<UserPreferences> GetUserPreferencesAsync(string id, string userId)
        {
            var response = await _client.GetAsync($"/UserPreferences/{id}/{userId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserPreferences>();
            }
            return null;
        }

        public async Task<Trip> CreateTripAsync(Trip trip)
        {
            var response = await _client.PostAsJsonAsync("/Dev/Trip", trip);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Trip>();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Error creating trip: {response.StatusCode} - {errorContent}");
                return null;
            }
        }
    }
}
