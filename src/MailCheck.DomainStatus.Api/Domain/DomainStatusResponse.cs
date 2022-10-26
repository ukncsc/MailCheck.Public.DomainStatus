using System.Collections.Generic;
using MailCheck.DomainStatus.Common.Dao;

namespace MailCheck.DomainStatus.Api.Domain
{
    public class DomainStatusResponse
    {
        public string Domain { get; set; }
        public Dictionary<string, Record> Records { get; set; }

        public DomainStatusResponse(string domain, Dictionary<string, Record> records)
        {
            Domain = domain;
            Records = records;
        }
    }
}