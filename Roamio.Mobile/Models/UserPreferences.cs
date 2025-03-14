using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roamio.Mobile.Services;

namespace Roamio.Mobile.Models
{
    public class UserPreferences
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public int EnergyLevel { get; set; }
        public List<string> ActivityPreferences { get; set; }
        public List<string> FoodPreferences { get; set; }
        public int MealsPerDay { get; set; }
    }
}
