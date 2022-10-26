using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using MailCheck.DomainStatus.Api.Controllers;
using MailCheck.DomainStatus.Api.Domain;
using MailCheck.DomainStatus.Api.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using A = FakeItEasy.A;

namespace MailCheck.DomainStatus.Api.Test.Controller
{
    [TestFixture]
    public class DomainStatusControllerTests
    {
        private DomainStatusController _domainStatusController;
        private IDomainStatusService _domainStatusService;
        private ILogger<DomainStatusController> _log;

        [SetUp]
        public void SetUp()
        {
            _domainStatusService = A.Fake<IDomainStatusService>();
            _log = A.Fake<ILogger<DomainStatusController>>();
            _domainStatusController = new DomainStatusController(_domainStatusService, _log);
        }

        [Test]
        public async Task ReturnsResultFromService()
        {
            var domains = new List<string>
            {
                "domainsdomains.com"
            };

            var responseFromService = new List<DomainStatusResponse>();
            A.CallTo(() => _domainStatusService.GetDomainStatuses(A<List<string>>.That.Matches(x => domains == x)))
                .Returns(responseFromService);

            var request = new LookupDomainStatusesRequest
            {
                Domains = domains
            };
            var result = (ObjectResult)await _domainStatusController.LookupDomainStatuses(request);

            Assert.AreEqual(responseFromService, result.Value);
        }
    }
}