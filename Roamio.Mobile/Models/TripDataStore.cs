using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roamio.Mobile.Models
{
    public static class TripDataStore
    {
        public static Trip CurrentTrip { get; set; } = new Trip();
    }
}
