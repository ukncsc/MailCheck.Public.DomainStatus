using System.Threading.Tasks;
using FakeItEasy;
using MailCheck.DomainStatus.Common.Dao;
using MailCheck.DomainStatus.Contracts;
using MailCheck.DomainStatus.Entity.Dao;
using MailCheck.DomainStatus.Entity.Entity;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace MailCheck.DomainStatus.Entity.Test
{
    public class DomainStatusEntityTests
    {
        private DomainStatusEvaluationHandler _handler;
        private ILogger<DomainStatusEntity> _logger;
        private IDomainStatusDao _domainStatusDao;

        [SetUp]
        public void SetUp()
        {
            _domainStatusDao = A.Fake<IDomainStatusDao>();
            _logger = A.Fake<ILogger<DomainStatusEntity>>();
            _handler = new DomainStatusEvaluationHandler(_domainStatusDao, _logger);
        }

        [Test]
        public async Task SavesCorrectDomainStatusDmarcRecordEvaluationsChanged()
        {
            DomainStatusEvaluation message = new DomainStatusEvaluation("testDomain", "testRecordType", (Status)999);

            await _handler.Handle(message);

            A.CallTo(() => _domainStatusDao.Save("testDomain", "testRecordType", (Status)999)).MustHaveHappenedOnceExactly();
        }
    }
}
