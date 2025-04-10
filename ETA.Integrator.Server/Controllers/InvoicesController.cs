using ETA.Integrator.Server.Interface;
using ETA.Integrator.Server.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;
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

            if (fromDate != null && toDate != null)
            {
                request.AddParameter("dateFrom", fromDate, ParameterType.QueryString)
                    .AddParameter("dateTo", toDate, ParameterType.QueryString);
            }

            var response = await _client.ExecuteAsync<List<InvoiceResponseModel>>(request);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, response.ErrorMessage);
            }

            return Ok(response);
        }
    }
}
