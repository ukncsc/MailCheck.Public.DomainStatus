using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using MailCheck.DomainStatus.Common.Dao;
using MailCheck.DomainStatus.Contracts;
using Microsoft.Extensions.Logging;
using Record = MailCheck.DomainStatus.Common.Dao.Record;

namespace MailCheck.DomainStatus.Entity.Dao
{
    public class ObjectModelDomainStatusDao : IDomainStatusDao
    {
        private readonly IDynamoDBContext _context;
        private readonly ILogger<ObjectModelDomainStatusDao> _log;

        public DynamoDBOperationConfig Config { get; set; }

        public ObjectModelDomainStatusDao(IDynamoDBContext context, ILogger<ObjectModelDomainStatusDao> log)
        {
            _context = context;
            _log = log;
        }

        public async Task Save(string domain, string recordType, Status status)
        {
            int count = 0;
            int maxTries = 3;
            while (true)
            {
                try
                {
                    DomainStatusEntity entity = await _context.LoadAsync<DomainStatusEntity>(domain, Config);

                    entity = entity ?? new DomainStatusEntity(domain, new Dictionary<string, Record>());
                    entity.Records[recordType] = new Record { RecordType = recordType, Status = status };

                    await _context.SaveAsync(entity, Config);
                    break;
                }
                catch (ConditionalCheckFailedException ex)
                {
                    if (++count == maxTries)
                    {
                        _log.LogError(ex, $"Optimistic concurrency exception occurred trying to save status for domain {domain} - attempt {count} of {maxTries}");
                        throw;
                    }
                    else
                    {
                        _log.LogWarning(ex, $"Optimistic concurrency exception occurred trying to save status for domain {domain} - attempt {count} of {maxTries}");
                    }
                }
            }
        }
    }
}