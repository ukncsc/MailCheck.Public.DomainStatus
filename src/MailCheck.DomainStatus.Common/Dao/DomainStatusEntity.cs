using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;

namespace MailCheck.DomainStatus.Common.Dao
{
    [DynamoDBTable("DomainStatusEntity")]
    public class DomainStatusEntity
    {
        public int SchemaVersion = 1;

        public DomainStatusEntity()
        {
        }

        public DomainStatusEntity(string domain, Dictionary<string, Record> records)
        {
            Domain = domain;
            Records = records;
        }

        [DynamoDBVersion]
        public long? Version { get; set; }

        [DynamoDBHashKey("Domain")]
        public string Domain { get; set; }

        public Dictionary<string, Record> Records { get; set; }
    }
}