using ETA.Integrator.Server.Dtos;
using ETA.Integrator.Server.Extensions;
using ETA.Integrator.Server.Interface.Services;
using ETA.Integrator.Server.Models.Consumer.ETA;
using ETA.Integrator.Server.Models.Consumer.Response;
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
        readonly RestClient _providerClient;
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

            if (_customConfig.Provider_APIURL != null)
            {
                var opt = new RestClientOptions(_customConfig.Provider_APIURL);
                _providerClient = new RestClient(opt);
            }
            else
            {
                throw new Exception("Error: Getting connection string");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetInvoices(DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var request = new RestRequest("/api/Invoices/GetInvoices", Method.Get);

                if (fromDate != null && toDate != null)
                {
                    request.AddParameter("fromDate", fromDate, ParameterType.QueryString)
                        .AddParameter("toDate", toDate, ParameterType.QueryString);
                }
                request.AddParameter("fromDate", ParameterType.QueryString)
                    .AddParameter("toDate", ParameterType.QueryString);

                var response = await _providerClient.ExecuteAsync<List<ProviderInvoiceViewModel>>(request);

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
            #region PreparingData
            InvoiceModel invoice = new InvoiceModel();

            //Issuer Data
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

            IssuerModel? issuerModel = issuerData.ToIssuerModel();

            #endregion
            return Ok(issuerModel);
        }
    }
}
