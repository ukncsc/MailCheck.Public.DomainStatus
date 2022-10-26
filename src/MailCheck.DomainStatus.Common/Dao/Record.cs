using Amazon.DynamoDBv2.DataModel;
using MailCheck.DomainStatus.Contracts;

namespace MailCheck.DomainStatus.Common.Dao
{
    public class Record
    {
        public string RecordType { get; set; }

        [DynamoDBProperty(typeof(StatusConverter))]
        public Status Status { get; set; }

        public Record()
        {
        }
    }
}