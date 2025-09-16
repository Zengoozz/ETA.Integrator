using ETA.Integrator.Server.Interface.Services.Common;
using ETA.Integrator.Server.Models;
using ETA.Integrator.Server.Models.Core;
using ETA.Integrator.Server.Models.Provider;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RestSharp;

namespace ETA.Integrator.Server.Controllers
{
    [Route("HMS/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly CustomConfigurations _customConfig;
        private readonly ILogger<InvoicesController> _logger;
        private readonly IApiCallerService _apiCallerService;

        public InvoicesController(
            IOptions<CustomConfigurations> customConfigurations,
            ILogger<InvoicesController> logger,
            IApiCallerService apiCallerService
            )
        {
            _logger = logger;
            _customConfig = customConfigurations.Value;
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
        public async Task<IActionResult> GetSubmissions(string submissionId, int pageNumber = 1, int pageSize = 100)
        {
            var response = await _apiCallerService.GetSubmission(submissionId, pageNumber, pageSize);

            return Ok(response);
        }

    }
}
