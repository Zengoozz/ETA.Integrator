using ETA.Integrator.Server.Dtos;
using ETA.Integrator.Server.Extensions;
using ETA.Integrator.Server.Interface.Services;
using ETA.Integrator.Server.Models.Consumer.ETA;
using ETA.Integrator.Server.Models.Consumer.Requests;
using ETA.Integrator.Server.Models.Consumer.Response;
using ETA.Integrator.Server.Models.Provider;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RestSharp;
using System.Text.Json;

namespace ETA.Integrator.Server.Controllers
{
    [Route("HMS/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly CustomConfigurations _customConfig;
        private readonly ILogger<InvoicesController> _logger;
        private readonly ISettingsStepService _settingsStepService;
        private readonly IConsumerService _consumerService;
        private readonly IRequestHandlerService _requestHandlerService;
        private readonly ISignatureService _signatureService;

        public InvoicesController(
            IOptions<CustomConfigurations> customConfigurations,
            ILogger<InvoicesController> logger,
            ISettingsStepService settingsStepService,
            IConsumerService consumerService,
            IRequestHandlerService requestHandlerService,
            ISignatureService signatureService
            )
        {
            _logger = logger;
            _customConfig = customConfigurations.Value;
            _settingsStepService = settingsStepService;
            _consumerService = consumerService;
            _requestHandlerService = requestHandlerService;
            _signatureService = signatureService;
        }
        [HttpGet]
        public async Task<IActionResult> GetProviderInvoices(DateTime? fromDate, DateTime? toDate)
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

                if (fromDate != null && toDate != null)
                {
                    request.AddParameter("fromDate", fromDate, ParameterType.QueryString)
                        .AddParameter("toDate", toDate, ParameterType.QueryString);
                }
                request.AddParameter("fromDate", ParameterType.QueryString)
                    .AddParameter("toDate", ParameterType.QueryString);

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

        [HttpPost("SubmitInvoice")]
        public async Task<IActionResult> SubmitInvoice(List<ProviderInvoiceViewModel> invoicesList)
        {

            var request = await _consumerService.SubmitInvoiceRequest(invoicesList);

            var response = await _requestHandlerService.ExecuteWithAuthRetryAsync(request);

            return Ok(response);
        }

        [HttpGet("GetRecent")]
        public async Task<IActionResult> GetRecentDocuments()
        {
            var request = _consumerService.GetRecentDocumentsRequest();

            var response = await _requestHandlerService.ExecuteWithAuthRetryAsync(request);

            return Ok(response.Content);
        }
        
        [HttpPost("GetSignature")]
        public async Task<IActionResult> GetSignature([FromBody] RootDocumentModel model)
        {
            var connectionSettings = await _settingsStepService.GetConnectionData();

            foreach (var invoice in model.Documents)
            {
                _signatureService.SignDocument(invoice, connectionSettings.TokenPin);
            }

            return Ok();
        }
    }
    public class RootDocumentModel
    {
        public List<InvoiceModel> Documents { get; set; } = new List<InvoiceModel>();
    }
}
