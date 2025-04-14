using ETA.Integrator.Server.Models.Provider;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace ETA.Integrator.Server.Controllers
{
    [Route("HMS/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        readonly RestClient _client;
        public InvoicesController()
        {
            var connectionString = Environment.GetEnvironmentVariable("HMS_API");

            if (connectionString != null)
            {
                var opt = new RestClientOptions(connectionString);
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

        [HttpPost("/HMS/Submit")]
        public async Task<IActionResult> SubmitInvoices(List<ProviderInvoiceViewModel> request)
        {
            return Ok();
        }
    }
}
