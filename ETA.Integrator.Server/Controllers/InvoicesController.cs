using ETA.Integrator.Server.Interface.Services.Common;
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

            return Ok(response);
        }

        [HttpPost("SubmitDocuments")]
        public async Task<IActionResult> SubmitDocuments(List<ProviderInvoiceViewModel> invoicesList)
        {
            var response = await _apiCallerService.SubmitDocuments(invoicesList);

            return Ok(response);
        }

        [HttpGet("GetRecent")]
        public async Task<IActionResult> GetRecentDocuments()
        {
            var response = await _apiCallerService.GetRecentDocuments();

            return Ok(response);
        }
    }
}
