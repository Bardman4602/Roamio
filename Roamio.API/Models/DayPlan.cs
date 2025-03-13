using Amazon.DynamoDBv2.DataModel;
using System.Collections.Generic;

namespace Roamio.API.Models
{
    [DynamoDBTable("DayPlans")]
    public class DayPlan
    {
        [DynamoDBHashKey] 
        public string TripId { get; set; }

        [DynamoDBRangeKey] 
        public string Date { get; set; }

        [DynamoDBProperty]
        public string Id { get; set; }

        [DynamoDBProperty]
        public Dictionary<string, object> Schedule { get; set; }

        [DynamoDBProperty]
        public List<string> StartTimes { get; set; }

        [DynamoDBProperty]
        public List<string> EndTimes { get; set; }

        [DynamoDBProperty]
        public List<string> Suggestions { get; set; }

        [DynamoDBProperty]
        public Dictionary<string, string> GoogleMapsData { get; set; }

        [DynamoDBProperty]
        public Dictionary<string, string> UserPreferences { get; set; }

        [DynamoDBProperty]
        public Dictionary<string, string> UserSelections { get; set; }
    }
}
