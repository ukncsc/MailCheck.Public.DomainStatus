using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using MailCheck.Common.Api.Middleware.Audit;
using MailCheck.DomainStatus.Api.Dao;
using MailCheck.DomainStatus.Api.Domain;
using MailCheck.DomainStatus.Api.Service;
using MailCheck.DomainStatus.Common.Dao;
using NUnit.Framework;
using DomainStatusEntity = MailCheck.DomainStatus.Common.Dao.DomainStatusEntity;

namespace MailCheck.DomainStatus.Api.Test.Service
{
    [TestFixture]
    public class DomainStatusServiceTest
    {
        private ILogger<DomainStatusEntity> _log;
        private IDomainStatusDao _domainStatusDao;
        private IDomainStatusService _domainStatusService;

        [SetUp]
        public void Setup()
        {
            _log = A.Fake<ILogger<DomainStatusEntity>>();
            _domainStatusDao = A.Fake<IDomainStatusDao>();
            _domainStatusService = new DomainStatusService(_domainStatusDao);
        }

        [Test]
        public async Task ReturnsResultFromDao()
        {
            List<string> domains = new List<string>
            {
                "testDomain"
            };
            List<DomainStatusEntity> resultFromDao = new List<DomainStatusEntity>
            {
                new DomainStatusEntity("testDomain", new Dictionary<string, Record>())
            };
            A.CallTo(() => _domainStatusDao.GetDomainsStatusInfo(domains))
                .Returns(resultFromDao);
            
            List<DomainStatusResponse> result = await _domainStatusService.GetDomainStatuses(domains);

            Assert.AreEqual(resultFromDao.Count, result.Count);
            Assert.AreEqual(resultFromDao[0].Domain, result[0].Domain);
            Assert.AreEqual(resultFromDao[0].Records, result[0].Records);
        }
    }
}
