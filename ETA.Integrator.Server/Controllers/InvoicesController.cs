using ETA.Integrator.Server.Interface.Services.Common;
using ETA.Integrator.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ETA.Integrator.Server.Controllers
{
    [Route("HMS/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly IApiCallerService _apiCallerService;

        public InvoicesController(
            IApiCallerService apiCallerService
            )
        {
            _apiCallerService = apiCallerService;
        }
        [HttpGet]
        public async Task<IActionResult> GetProviderInvoices(DateTime? fromDate, DateTime? toDate, string invoiceType = "")
        {
            var response = await _apiCallerService.GetProviderInvoices(fromDate, toDate, invoiceType);

            return Ok(response.OrderBy(r => r.InvoiceNumber));
        }

        [HttpPost("SubmitDocuments")]
        public async Task<IActionResult> SubmitDocuments(InvoiceRequest request)
        {
            var response = await _apiCallerService.SubmitDocuments(request);

            return Ok(response);
        }

        [HttpGet("GetRecent")]
        public async Task<IActionResult> GetRecentDocuments()
        {
            var response = await _apiCallerService.GetRecentDocuments();

            return Ok(response);
        }

        [HttpGet("GetSubmissions")]
        public async Task<IActionResult> GetSubmissions(string submissionId, int pageNo = 1, int pageSize = 100)
        {
            var response = await _apiCallerService.GetSubmission(submissionId, pageNo, pageSize);

            return Ok(response);
        }

    }
}
