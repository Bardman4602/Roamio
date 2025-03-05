using Amazon.DynamoDBv2.DataModel;
using System.Collections.Generic;

namespace Roamio.API.Models
{
    [DynamoDBTable("UserPreferences")]
    public class UserPreferences
    {
        [DynamoDBHashKey] 
        public string Id { get; set; }

        [DynamoDBRangeKey] 
        public string UserId { get; set; }

        [DynamoDBProperty]
        public int EnergyLevel { get; set; }

        [DynamoDBProperty]
        public List<string> ActivityPreferences { get; set; }

        [DynamoDBProperty]
        public List<string> FoodPreferences { get; set; }
    }
}
