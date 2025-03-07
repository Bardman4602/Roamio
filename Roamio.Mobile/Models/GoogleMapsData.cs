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
}
