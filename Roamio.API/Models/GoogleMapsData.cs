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
        public Dictionary<string, object> Places { get; set; }

        [DynamoDBProperty]
        public Dictionary<string, object> Navigation { get; set; }

        [DynamoDBProperty]
        public Dictionary<string, object> GeoCoding { get; set; }

        [DynamoDBProperty]
        public Dictionary<string, object> PublicTransport { get; set; }
    }
}
