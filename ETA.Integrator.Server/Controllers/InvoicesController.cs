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

        public InvoicesController(
            IOptions<CustomConfigurations> customConfigurations,
            ILogger<InvoicesController> logger,
            ISettingsStepService settingsStepService,
            IInvoiceService invoiceService
            )
        {
            _logger = logger;
            _customConfig = customConfigurations.Value;
            _settingsStepService = settingsStepService;
            _invoiceService = invoiceService;
        }
        [HttpGet]
        public async Task<IActionResult> GetInvoices(DateTime? fromDate, DateTime? toDate)
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
            #region API_CONNECT_CONFIG

            var connectionConfig = await _settingsStepService.GetConnectionData();

            // CLIENT_ID | CLIENT_SECRET VALIDATION
            if (connectionConfig == null || string.IsNullOrWhiteSpace(connectionConfig.ClientId) || string.IsNullOrWhiteSpace(connectionConfig.ClientSecret))
            {
                _logger.LogError("Failed to get the manual connection config");

                return StatusCode(StatusCodes.Status400BadRequest, new GenericResponse<string>
                {
                    Success = false,
                    Code = "CONNECTION_CONFIG_NOT_FOUND",
                    Message = "Connection configuration (Manual) not found",
                    Data = null
                });
            }

            var token = "";

            // GENERATING CONSUMER TOKEN
            if (!String.IsNullOrWhiteSpace(_customConfig.Consumer_IdSrvBaseUrl))
            {
                var authOpt = new RestClientOptions(_customConfig.Consumer_IdSrvBaseUrl);
                var authClient = new RestClient(authOpt);

                var authRequest = new RestRequest("/connect/token", Method.Post)
                    .AddParameter("grant_type", "client_credentials")
                    .AddParameter("client_id", connectionConfig.ClientId)
                    .AddParameter("client_secret", connectionConfig.ClientSecret)
                    .AddParameter("scope", "InvoicingAPI");

                var response = await authClient.ExecuteAsync<ConsumerConnectionResponseModel>(authRequest);

                if (response.Data != null && response.IsSuccessStatusCode)
                {
                    token = response.Data.access_token;
                }
                else
                {
                    _logger.LogError("Failed to connect to the consumer API");
                    return StatusCode(StatusCodes.Status500InternalServerError, new GenericResponse<RestResponse<ConsumerConnectionResponseModel>?>
                    {
                        Success = false,
                        Code = "CONSUMER_CONNECTION_FAILED",
                        Message = "Connecting to consumer failed",
                        Data = response
                    });
                }
            }
            else
            {
                _logger.LogError("Failed to get the consumer IdSrvURL");
                return StatusCode(StatusCodes.Status500InternalServerError, new GenericResponse<string>
                {
                    Success = false,
                    Code = "CONNECTION_URL_NOT_FOUND",
                    Message = "Connection configuration (IdSrvUrl) not found",
                    Data = null
                });
            }
            #endregion

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

            #region SUBMIT

            var authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(token, "Bearer");

            var submitOpt = new RestClientOptions(_customConfig.Consumer_APIBaseUrl)
            {
                Authenticator = authenticator
            };

            var submitClient = new RestClient(submitOpt);

            var submitRequestBody = new
            {
                documents = documents
            };

            var submitRequest = new RestRequest("/api/v1/documentsubmissions", Method.Post)
                .AddHeader("Content-Type", "application/json").AddJsonBody(submitRequestBody);


            var submitResponse = await submitClient.ExecuteAsync(submitRequest);

            #endregion

            return Ok(submitRequestBody);
        }
    }
}
