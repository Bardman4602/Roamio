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
}
