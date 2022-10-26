using MailCheck.Common.Messaging.Abstractions;

namespace MailCheck.DomainStatus.Contracts
{
    public class DomainStatusEvaluation : Message
    {
        public string RecordType { get; }

        public Status Status { get; }

        public DomainStatusEvaluation(string id, string recordType, Status status) : base(id)
        {
            RecordType = recordType;
            Status = status;
        }
    }
}
