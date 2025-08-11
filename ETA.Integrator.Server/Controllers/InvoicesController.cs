using ETA.Integrator.Server.Interface.Services.Consumer;
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
        private readonly IApiConsumerService _apiConsumerService;

        public InvoicesController(
            IOptions<CustomConfigurations> customConfigurations,
            ILogger<InvoicesController> logger,
            IApiConsumerService apiConsumerService
            )
        {
            _logger = logger;
            _customConfig = customConfigurations.Value;
            _apiConsumerService = apiConsumerService;
        }
        [HttpGet]
        public async Task<IActionResult> GetProviderInvoices(DateTime? fromDate, DateTime? toDate, string invoiceType = "")
        {
            try
            {
                var client = new RestClient();

                if (!String.IsNullOrWhiteSpace(_customConfig.Provider_APIURL))
                {
                    var opt = new RestClientOptions(_customConfig.Provider_APIURL);
                    client = new RestClient(opt);
                }
                else
                {
                    throw new Exception("Error: Getting provider API_URL");
                }

                var request = new RestRequest("/api/Invoices/GetInvoices", Method.Get);

                if (fromDate != null && toDate != null && !string.IsNullOrWhiteSpace(invoiceType))
                {
                    request.AddParameter("fromDate", fromDate, ParameterType.QueryString)
                        .AddParameter("toDate", toDate, ParameterType.QueryString)
                        .AddParameter("invoiceType", invoiceType, ParameterType.QueryString);
                }
                else
                {
                    return BadRequest("Please provide fromDate, toDate and invoiceType parameters.");
                }

                var response = await client.ExecuteAsync<List<ProviderInvoiceViewModel>>(request);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, response.ErrorMessage);
                }
                return Ok(response.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while retrieving invoices");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occured while retrieving invoices");
            }
        }

        [HttpPost("SubmitDocuments")]
        public async Task<IActionResult> SubmitDocuments(List<ProviderInvoiceViewModel> invoicesList)
        {
            try
            {
                var response = await _apiConsumerService.SubmitDocuments(invoicesList);

                return StatusCode(response.StatusCode, response.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while submitting invoices");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        [HttpGet("GetRecent")]
        public async Task<IActionResult> GetRecentDocuments()
        {
            var response = await _apiConsumerService.GetRecentDocuments();

            return Ok(response);
        }
    }
}
