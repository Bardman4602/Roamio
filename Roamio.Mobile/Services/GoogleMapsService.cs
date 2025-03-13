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
    }
}
