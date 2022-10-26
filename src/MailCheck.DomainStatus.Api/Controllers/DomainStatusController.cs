using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailCheck.Common.Api.Authorisation.Filter;
using MailCheck.Common.Api.Authorisation.Service.Domain;
using MailCheck.Common.Api.Domain;
using MailCheck.DomainStatus.Api.Domain;
using MailCheck.DomainStatus.Api.Service;
using MailCheck.DomainStatus.Api.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MailCheck.DomainStatus.Api.Controllers
{
    [Route("api/domain-status")]
    public class DomainStatusController : Controller
    {
        private readonly IDomainStatusService _domainStatusService;
        private readonly ILogger<DomainStatusController> _log;

        public DomainStatusController(IDomainStatusService domainStatusService, ILogger<DomainStatusController> log)
        {
            _domainStatusService = domainStatusService;
            _log = log;
        }

        [HttpPost("lookup")]
        [MailCheckAuthoriseRole(Role.Standard)]
        public async Task<IActionResult> LookupDomainStatuses([FromBody]LookupDomainStatusesRequest request)
        {
            if (!ModelState.IsValid)
            {
                _log.LogWarning($"Bad request: {ModelState.GetErrorString()}");
                return BadRequest(new ErrorResponse(ModelState.GetErrorString()));
            }

            var getDomains = await _domainStatusService.GetDomainStatuses(request.Domains);

            return new ObjectResult(getDomains);
        }
    }

    public class LookupDomainStatusesRequest
    {
        public List<string> Domains { get; set; }
    }
}