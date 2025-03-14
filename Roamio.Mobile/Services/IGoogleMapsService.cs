using System.Threading.Tasks;
using Roamio.Mobile.Models;

namespace Roamio.Mobile.Services
{
    public interface IGoogleMapsService
    {
        Task<GeocodeResponse> GetGeocodeAsync(string address);
        Task<List<SuggestionItem>> GetRestaurantSuggestionsAsync(string destination);
        Task<List<SuggestionItem>> GetActivitySuggestionsAsync(string destination);
    }
}
