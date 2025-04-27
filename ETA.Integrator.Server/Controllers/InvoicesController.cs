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

        public InvoicesController(
            IOptions<CustomConfigurations> customConfigurations,
            ILogger<InvoicesController> logger,
            ISettingsStepService settingsStepService
            )
        {
            //var connectionString = Environment.GetEnvironmentVariable("HMS_API");
            _logger = logger;
            _customConfig = customConfigurations.Value;
            _settingsStepService = settingsStepService;
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
        public async Task<IActionResult> SubmitInvoice(ProviderInvoiceViewModel invoiceViewModel)
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

            #region PREPARING_DATA

            InvoiceModel document = new InvoiceModel();

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
                return StatusCode(StatusCodes.Status500InternalServerError ,new GenericResponse<IssuerDTO>
                {
                    Success = false,
                    Code = "ISSUER_MAPPING_FAILED",
                    Message = "Failed to map Issuer Data!",
                    Data = issuerData
                });
            }
            #endregion

            #region RECEIVER_PREP

            // BUILDING NUMBER | STREET | REGION/CITY | GOVERNATE | COUNTRY
            List<string> address = invoiceViewModel.ReceiverAddress.Split('-').ToList();

            ReceiverModel receiver = new ReceiverModel();

            receiver.Type = "B";
            receiver.Id = invoiceViewModel.RegistrationNumber;
            receiver.Name = invoiceViewModel.ReceiverName;
            receiver.Address = new ReceiverAddressModel
            {
                BuildingNumber = address[0],
                Street = address[1],
                RegionCity = address[2],
                Governate = address[3],
                Country = address[4]
            };

            #endregion

            #region INVOICE_LINE_PREP

            List<InvoiceLineModel> invoiceLineList = new List<InvoiceLineModel>();

            InvoiceLineModel invoiceLine = new InvoiceLineModel
            {
                InternalCode = "" // REQUIRED
            };

            invoiceLineList.Add(invoiceLine);

            #endregion

            #region TAX_TOTAL_PREP

            List<TaxTotalModel> taxTotalList = new List<TaxTotalModel>();

            //TaxTotalModel taxTotalModel = new TaxTotalModel();

            //taxTotalList.Add(taxTotalModel);

            #endregion

            #region SIGNATURES_PREP

            List<SignatureModel> signatureList = new List<SignatureModel>();

            //SignatureModel signature = new SignatureModel
            //{
            //    Type = "S",
            //    //Value = "SignatureValue", // REQUIRED
            //};

            //signatureList.Add(signature);

            #endregion

            #region INVOICE_PREP

            document.Issuer = issuer;
            document.Receiver = receiver;
            document.DocumentType = "i";
            document.DocumentTypeVersion = "1.0";
            document.DateTimeIssued = invoiceViewModel.CreatedDate;
            document.TaxpayerActivityCode = "8610"; // HOSPITAL ACTIVITIES CODE
            document.InternalId = invoiceViewModel.InvoiceId.ToString();
            document.InvoiceLines = invoiceLineList;
            document.NetAmount = invoiceViewModel.NetPrice;
            document.TaxTotals = taxTotalList;
            document.Signatures = signatureList;
            document.TotalSalesAmount = 0; // SUM INVOICE LINES SALES
            document.TotalDiscountAmount = 0; // SUM INVOICE LINES DISCOUNTS
            document.TotalItemsDiscountAmount = 0; // ? SAME AS TOTAL DISCOUNT AMOUNT ????
            document.ExtraDiscountAmount = 0; // DISCOUNT OVERALL DOCUMENT
            document.TotalAmount = invoiceViewModel.NetPrice + taxTotalList.Sum(x => x.Amount); // NET + TOTAL TAX
            //document.purchaseOrderReference = ; // OPTIONAL
            //document.purchaseOrderDescription = ; // OPTIONAL
            //document.salesOrderReference = ; // OPTIONAL
            //document.salesOrderDescription = ; // OPTIONAL
            //document.proformaInvoiceNumber = ; // OPTIONAL
            //document.payment = ; // OPTIONAL
            //document.delivery = ; // OPTIONAL
            //document.ServiceDeliveryDate = ; //OPTIONAL

            #endregion

            #endregion

            #region SUBMIT

            var authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(token, "Bearer");

            var submitOpt = new RestClientOptions(_customConfig.Consumer_APIBaseUrl)
            {
                Authenticator = authenticator
            };

            var submitClient = new RestClient(submitOpt);


            var submitRequest = new RestRequest("/api/v1/documentsubmissions", Method.Post)
                .AddBody(document);


            #endregion

            return Ok(issuer);
        }
    }
}
