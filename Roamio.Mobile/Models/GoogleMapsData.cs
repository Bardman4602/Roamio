using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roamio.Mobile.Services;

namespace Roamio.Mobile.Models
{
    public class GoogleMapsData
    {
        public string UserId { get; set; }
        public string Location { get; set; }
        public Dictionary<string, object> Places { get; set; }
        public Dictionary<string, object> Navigation { get; set; }
        public Dictionary<string, object> GeoCoding { get; set; }
        public Dictionary<string, object> PublicTransport { get; set; }
    }

    public class GeocodeResponse
    {
        public string status { get; set; }
        public GeocodeResult[] results { get; set; }
    }

    public class GeocodeResult
    {
        public string formatted_address { get; set; }
        public Geometry geometry { get; set; }
    }

    public class Geometry
    {
        public Location location { get; set; }
    }

    public class Location
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class SuggestionItem
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Rating { get; set; }
        public string Description { get; set; }
    }

    public class PlacesResponse
    {
        public string Status { get; set; }
        public PlaceResult[] Results { get; set; }
    }

    public class PlaceResult
    {
        public string Name { get; set; }
        public string FormattedAddress { get; set; }
        public double Rating { get; set; }
        public string[] Types { get; set; }
    }
}
