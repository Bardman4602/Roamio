using Amazon.DynamoDBv2.DataModel;
using System.Collections.Generic;

namespace Roamio.API.Models
{
    [DynamoDBTable("GoogleMapsData")]
    public class GoogleMapsData
    {
        [DynamoDBHashKey] 
        public string UserId { get; set; }

        [DynamoDBRangeKey] 
        public string Location { get; set; }

        [DynamoDBProperty]
        public Dictionary<string, string> Places { get; set; }

        [DynamoDBProperty]
        public Dictionary<string, string> Navigation { get; set; }

        [DynamoDBProperty]
        public Dictionary<string, string> GeoCoding { get; set; }

        [DynamoDBProperty]
        public Dictionary<string, string> PublicTransport { get; set; }
    }
}
