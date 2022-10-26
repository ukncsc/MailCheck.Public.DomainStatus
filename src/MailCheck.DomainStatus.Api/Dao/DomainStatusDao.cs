using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using MailCheck.DomainStatus.Common.Dao;

namespace MailCheck.DomainStatus.Api.Dao
{
    public interface IDomainStatusDao
    {
        Task<List<DomainStatusEntity>> GetDomainsStatusInfo(List<string> domains);
    }

    public class DomainStatusDao : IDomainStatusDao
    {
        private readonly IDynamoDBContext _context;
        public DynamoDBOperationConfig Config { get; set; }

        public DomainStatusDao(IDynamoDBContext context)
        {
            _context = context;
        }

        public async Task<List<DomainStatusEntity>> GetDomainsStatusInfo(List<string> domains)
        {
            BatchGet<DomainStatusEntity> domainBatch = _context.CreateBatchGet<DomainStatusEntity>(Config);

            foreach (string domain in domains)
            {
                domainBatch.AddKey(domain);
            }

            await domainBatch.ExecuteAsync();

            var results = domainBatch.Results ?? new List<DomainStatusEntity>();

            return results;
        }
    }
}