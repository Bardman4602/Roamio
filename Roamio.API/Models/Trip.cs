using Amazon.DynamoDBv2.DataModel;
using System.Collections.Generic;

namespace Roamio.API.Models
{
    [DynamoDBTable("Trips")]
    public class Trip
    {
        [DynamoDBHashKey] 
        public string Id { get; set; }

        [DynamoDBRangeKey] 
        public string UserId { get; set; }

        [DynamoDBProperty]
        public string Destination { get; set; }

        [DynamoDBProperty]
        public string StartDate { get; set; }

        [DynamoDBProperty]
        public string EndDate { get; set; }

        [DynamoDBProperty]
        public Dictionary<string, object> UserPreferences { get; set; }

        [DynamoDBProperty]
        public Dictionary<string, object> GoogleMapsData { get; set; }

        [DynamoDBProperty]
        public List<string> DailyPlans { get; set; }
    }
}
