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
        readonly RestClient _client;
        private readonly CustomConfigurations _customConfig;

        private readonly ILogger<InvoicesController> _logger;

        public InvoicesController (
            IOptions<CustomConfigurations> customConfigurations,
            ILogger<InvoicesController> logger
            )
        {
            //var connectionString = Environment.GetEnvironmentVariable("HMS_API");
            _logger = logger;
            _customConfig = customConfigurations.Value;

            if (_customConfig.API_URL != null)
            {
                var opt = new RestClientOptions(_customConfig.API_URL);
                _client = new RestClient(opt);
            }
            else
            {
                throw new Exception("Error: Getting connection string");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetInvoices(DateTime? fromDate, DateTime? toDate)
        {
            var request = new RestRequest("/api/Invoices/GetInvoices", Method.Get);

            request.AddParameter("fromDate", fromDate, ParameterType.QueryString)
                .AddParameter("toDate", toDate, ParameterType.QueryString);

            var response = await _client.ExecuteAsync<List<ProviderInvoiceViewModel>>(request);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, response.ErrorMessage);
            }

            return Ok(response.Data);
        }

        [HttpPost("Submit")]
        public async Task<IActionResult> SubmitInvoices(List<ProviderInvoiceViewModel> request)
        {
            return Ok();
        }
    }
}
