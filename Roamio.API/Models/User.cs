using Amazon.DynamoDBv2.DataModel;
using System.Collections.Generic;

namespace Roamio.API.Models
{
    [DynamoDBTable("Users")]
    public class User
    {
        [DynamoDBHashKey] // Partition key
        public string Id { get; set; }

        [DynamoDBProperty]
        public string Username { get; set; }

        [DynamoDBProperty]
        public string HashedPassword { get; set; }

        [DynamoDBProperty]
        public Dictionary<string, object> Preferences { get; set; }

        [DynamoDBProperty]
        public string CurrentTrip { get; set; }

        [DynamoDBProperty]
        public List<string> TripHistory { get; set; }

        [DynamoDBProperty]
        public string DailyPlans { get; set; }
    }
}
