using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.DataModel;

namespace Common.Models
{
    [DynamoDBTable("pushSubscribers-beta")]
    public class PushSubscriptionTable
    {
        [DynamoDBHashKey]
        public string Id { get; set; }
        
        public string PushSubscription { get; set; }
    }

    public class PushSubscriptionModel
    {
        public string Endpoint { get; set; }
        public int? ExpirationTime { get; set; }
        public Keys Keys { get; set; }
    }

    public class Keys
    {
        public string P256dh { get; set; }
        public string Auth { get; set; }
    }
}
