using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailCheck.DomainStatus.Api.Dao;
using MailCheck.DomainStatus.Api.Domain;
using MailCheck.DomainStatus.Common.Dao;

namespace MailCheck.DomainStatus.Api.Service
{
    public interface IDomainStatusService
    { 
        Task<List<DomainStatusResponse>> GetDomainStatuses(List<string> domains);
    }

    public class DomainStatusService : IDomainStatusService
    {
        private readonly IDomainStatusDao _domainStatusApiDao;

        public DomainStatusService(IDomainStatusDao domainStatusApiDao)
        {
            _domainStatusApiDao = domainStatusApiDao;
        }

        public async Task<List<DomainStatusResponse>> GetDomainStatuses(List<string> domains)
        {
            List<DomainStatusEntity> domainStatusEntities = await _domainStatusApiDao.GetDomainsStatusInfo(domains);

            var results = domainStatusEntities
                .Select(x => new DomainStatusResponse(x.Domain, x.Records))
                .ToList();

            return results;
        }
    }
}

