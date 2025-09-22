using ETA.Integrator.Server.Dtos;
using ETA.Integrator.Server.Interface.Services.Common;
using ETA.Integrator.Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace ETA.Integrator.Server.Controllers
{
    [Route("HMS/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly IApiCallerService _apiCallerService;

        public InvoicesController(
            IApiCallerService apiCallerService
            )
        {
            _apiCallerService = apiCallerService;
        }
        [HttpGet]
        public async Task<IActionResult> GetProviderInvoices(DateTime fromDate, DateTime toDate, string invoiceType, List<string> invoicesIds)
        {
            ProviderInvoicesSearchDTO searchModel = new ProviderInvoicesSearchDTO()
            {
                StartDate = fromDate,
                EndDate = toDate,
                InvoiceType = string.IsNullOrEmpty(invoiceType) ? "I" : invoiceType,
                InvoicesIds = invoicesIds
            };
            var response = await _apiCallerService.GetProviderInvoices(searchModel);

            return Ok(response.OrderBy(r => r.InvoiceNumber));
        }

        [HttpPost("SubmitDocuments")]
        public async Task<IActionResult> SubmitDocuments(InvoiceRequest request)
        {
            var response = await _apiCallerService.SubmitDocuments(request);

            return Ok(response);
        }

        [HttpGet("GetRecent")]
        public async Task<IActionResult> GetRecentDocuments()
        {
            var response = await _apiCallerService.GetRecentDocuments();

            return Ok(response);
        }

        [HttpGet("GetSubmissions")]
        public async Task<IActionResult> GetSubmissions(string submissionId, int pageNo = 1, int pageSize = 100)
        {
            var response = await _apiCallerService.GetSubmission(submissionId, pageNo, pageSize);

            return Ok(response);
        }

        [HttpGet("SearchDocuments")]
        public async Task<IActionResult> SearchDocuments(DateTime submissionDateFrom, DateTime submissionDateTo, string status, string receiverType, string direction = "Sent")
        {
            var response = await _apiCallerService.SearchDocuments(submissionDateFrom, submissionDateTo, status, receiverType, direction);

            return Ok(response);
        }
    }
}
