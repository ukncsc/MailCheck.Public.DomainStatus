using System.Threading.Tasks;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.DomainStatus.Common.Dao;
using MailCheck.DomainStatus.Contracts;
using MailCheck.DomainStatus.Entity.Dao;
using Microsoft.Extensions.Logging;

namespace MailCheck.DomainStatus.Entity.Entity
{
    public class DomainStatusEvaluationHandler : IHandle<DomainStatusEvaluation>
    {
        private readonly IDomainStatusDao _domainStatusDao;
        private readonly ILogger<DomainStatusEntity> _log;

        public DomainStatusEvaluationHandler(IDomainStatusDao domainStatusDao, ILogger<DomainStatusEntity> log)
        {
            _domainStatusDao = domainStatusDao;
            _log = log;
        }

        public async Task Handle(DomainStatusEvaluation message)
        {
            _log.LogInformation($"Received {message.RecordType} DomainStatusEvaluation for domain {message.Id} of {message.Status.ToString()}", message);

            await _domainStatusDao.Save(message.Id, message.RecordType, message.Status);
        }
    }
}