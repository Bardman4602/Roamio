using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Roamio.Mobile.Models;
using Microsoft.Extensions.Configuration;

namespace Roamio.Mobile.Services
{
    public class GoogleMapsService : IGoogleMapsService
    {
        private readonly HttpClient _httpClient;
        //private readonly string _apiKey;
        private readonly string _apiKey = "AIzaSyBqDN4XjKJduNIl7mnjVN7d5yU_xCg5xWE";  // Terrible practice. I know.

        public GoogleMapsService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("GoogleMapsClient");
            //_apiKey = configuration["GoogleMaps:ApiKey"];
        }
        

        public async Task<GeocodeResponse> GetGeocodeAsync(string address)
        {
            var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={_apiKey}";
            System.Diagnostics.Debug.WriteLine($"Geocode Request: {url}");
            return await _httpClient.GetFromJsonAsync<GeocodeResponse>(url);
        }

        public async Task<List<SuggestionItem>> GetRestaurantSuggestionsAsync(string destination, List<string> foodPrefs)
        {
            //string query = $"Things to do in {destination}";
            //string url = $"https://maps.googleapis.com/maps/api/place/textsearch/json?query={Uri.EscapeDataString(query)}&key={_apiKey}";
            //System.Diagnostics.Debug.WriteLine($"Activity Suggestions Request: {url}");

            //var placesResponse = await _httpClient.GetFromJsonAsync<PlacesResponse>(url);
            //if (placesResponse != null && placesResponse.Status == "OK" && placesResponse.Results != null)
            //{
            //    var suggestions = placesResponse.Results.Select(r => new SuggestionItem
            //    {
            //        Name = r.Name,
            //        Address = r.FormattedAddress,
            //        Rating = r.Rating.ToString(),
            //        Description = r.Types != null ? string.Join(", ", r.Types) : ""
            //    }).ToList();
            //    return suggestions;
            //}
            //return new List<SuggestionItem>();

            var allResults = new List<SuggestionItem>();
            foreach (var pref in foodPrefs)
            {
                string query = $"{pref} restaurants in {destination}";
                var places = await FetchPlacesAsync(query);
                allResults.AddRange(places);
            }

            // remove duplicates by name
            allResults = allResults
                .GroupBy(r => r.Name)
                .Select(g => g.First())
                .ToList();

            return allResults;
        }

        public async Task<List<SuggestionItem>> GetActivitySuggestionsAsync(string destination, List<string> activityPrefs)
        {
            //string query = $"Restaurants in {destination}";
            //string url = $"https://maps.googleapis.com/maps/api/place/textsearch/json?query={Uri.EscapeDataString(query)}&key={_apiKey}";
            //System.Diagnostics.Debug.WriteLine($"Restaurant Suggestions Request: {url}");

            //var placesResponse = await _httpClient.GetFromJsonAsync<PlacesResponse>(url);
            //if (placesResponse != null && placesResponse.Status == "OK" && placesResponse.Results != null)
            //{
            //    var suggestions = placesResponse.Results.Select(r => new SuggestionItem
            //    {
            //        Name = r.Name,
            //        Address = r.FormattedAddress,
            //        Rating = r.Rating.ToString(),
            //        Description = r.Types != null ? string.Join(", ", r.Types) : ""
            //    }).ToList();
            //    return suggestions;
            //}
            //return new List<SuggestionItem>();

            var allResults = new List<SuggestionItem>();
            foreach (var pref in activityPrefs)
            {
                string query = $"{pref} in {destination}";
                var places = await FetchPlacesAsync(query);
                allResults.AddRange(places);
            }

            // remove duplicates by name
            allResults = allResults
                .GroupBy(r => r.Name)
                .Select(g => g.First())
                .ToList();

            return allResults;
        }

        private async Task<List<SuggestionItem>> FetchPlacesAsync(string query)
        {
            string url = $"https://maps.googleapis.com/maps/api/place/textsearch/json?query={Uri.EscapeDataString(query)}&key={_apiKey}";
            var response = await _httpClient.GetFromJsonAsync<PlacesResponse>(url);
            if (response?.Status == "OK" && response.Results != null)
            {
                return response.Results.Select(r => new SuggestionItem
                {
                    Name = r.Name,
                    Address = r.FormattedAddress,
                    Rating = r.Rating.ToString(),
                    Description = r.Types != null ? string.Join(", ", r.Types) : ""
                }).ToList();
            }
            return new List<SuggestionItem>();
        }
    }
}
