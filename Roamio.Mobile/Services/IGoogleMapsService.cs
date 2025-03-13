using System.Threading.Tasks;
using Roamio.Mobile.Models;

namespace Roamio.Mobile.Services
{
    public interface IGoogleMapsService
    {
        Task<GeocodeResponse> GetGeocodeAsync(string address);
    }
}
