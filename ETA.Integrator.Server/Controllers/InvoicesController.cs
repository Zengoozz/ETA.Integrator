using ETA.Integrator.Server.Dtos;
using ETA.Integrator.Server.Extensions;
using ETA.Integrator.Server.Interface.Services;
using ETA.Integrator.Server.Models.Consumer.ETA;
using ETA.Integrator.Server.Models.Consumer.Response;
using ETA.Integrator.Server.Models.Provider;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RestSharp;
using RestSharp.Authenticators.OAuth2;
using System.Net;

namespace ETA.Integrator.Server.Controllers
{
    [Route("HMS/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly CustomConfigurations _customConfig;
        private readonly ILogger<InvoicesController> _logger;
        private readonly ISettingsStepService _settingsStepService;
        private readonly IInvoiceService _invoiceService;
        private readonly IConsumerRequestsHandlerService _consumerRequestHandlerService;

        public InvoicesController(
            IOptions<CustomConfigurations> customConfigurations,
            ILogger<InvoicesController> logger,
            ISettingsStepService settingsStepService,
            IInvoiceService invoiceService,
            IConsumerRequestsHandlerService consumerRequestHandlerService
            )
        {
            _logger = logger;
            _customConfig = customConfigurations.Value;
            _settingsStepService = settingsStepService;
            _invoiceService = invoiceService;
            _consumerRequestHandlerService = consumerRequestHandlerService;
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
        public async Task<IActionResult> SubmitInvoice(List<ProviderInvoiceViewModel> ivoiceViewModelList)
        {
            List<InvoiceModel> documents = new List<InvoiceModel>();

            #region ISSUER_PREP

            IssuerDTO? issuerData = await _settingsStepService.GetIssuerData();

            if (issuerData == null)
            {
                return BadRequest(new GenericResponse<string>
                {
                    Success = false,
                    Code = "ISSUER_NOT_FOUND",
                    Message = "Issuer data not found!",
                    Data = null
                });
            }

            IssuerModel? issuer = issuerData.ToIssuerModel();

            if (issuer == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new GenericResponse<IssuerDTO>
                {
                    Success = false,
                    Code = "ISSUER_MAPPING_FAILED",
                    Message = "Failed to map Issuer Data!",
                    Data = issuerData
                });
            }
            #endregion

            #region INVOICE_PREP
            foreach (var invoice in ivoiceViewModelList)
            {
                var doc = _invoiceService.PrepareInvoiceData(invoice, issuer);

                documents.Add(doc);
            }
            #endregion

            var submitRequestBody = new
            {
                documents = documents
            };

            var submitRequest = new RestRequest("/api/v1/documentsubmissions", Method.Post)
                .AddHeader("Content-Type", "application/json").AddJsonBody(submitRequestBody);

            var submitResponse = await _consumerRequestHandlerService.ExecuteWithAuthRetryAsync(submitRequest);

            return Ok(submitResponse);
        }

        [HttpGet("GetRecent")]
        public async Task<IActionResult> GetRecentDocuments()
        {
            DateTime utcNow = DateTime.UtcNow;

            // Create a new DateTime without milliseconds
            DateTime trimmedUtcNow = new DateTime(
                utcNow.Year,
                utcNow.Month,
                utcNow.Day,
                utcNow.Hour,
                utcNow.Minute,
                utcNow.Second,
                DateTimeKind.Utc
            );

            var request = new RestRequest("/api/v1/documents/recent", Method.Get)
                .AddHeader("Content-Type", "application/json")
                .AddQueryParameter("pageNo", 1)
                .AddQueryParameter("pageSize", 100)
                .AddQueryParameter("submissionDateFrom", trimmedUtcNow.AddMonths(-1).ToString())
                .AddQueryParameter("submissionDateTo", trimmedUtcNow.ToString())
                .AddQueryParameter("documentType", "i");

            var response = await _consumerRequestHandlerService.ExecuteWithAuthRetryAsync(request);

            return Ok(response.Data.Content);
        }

    }
}
