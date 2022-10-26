using System.Threading.Tasks;
using MailCheck.DomainStatus.Contracts;

namespace MailCheck.DomainStatus.Entity.Dao
{
    public interface IDomainStatusDao
    {
        Task Save(string domain, string recordType, Status status);
    }
}