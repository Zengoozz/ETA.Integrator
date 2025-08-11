using ETA.Integrator.Server.Data;
using ETA.Integrator.Server.Dtos.ConsumerAPI.GetRecentDocuments;
using ETA.Integrator.Server.Dtos.ConsumerAPI.SubmitDocuments;
using ETA.Integrator.Server.Interface.Services;
using ETA.Integrator.Server.Interface.Services.Consumer;
using ETA.Integrator.Server.Models.Provider;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace ETA.Integrator.Server.Services.Consumer
{
    public class ApiConsumerService : IApiConsumerService
    {
        private readonly IRequestFactoryConsumerService _requestFactoryConsumerService;
        private readonly IHttpRequestSenderConsumerService _httpRequestSenderConsumerService;
        private readonly IResponseProcessorConsumerService _responseProcessorConsumerService;
        private readonly IInvoiceSubmissionLogService _invoiceSubmissionLogService;
        private readonly AppDbContext _context;
        public ApiConsumerService(
            IRequestFactoryConsumerService requestBuilderConsumerService,
            IHttpRequestSenderConsumerService httpClientConsumerService,
            IResponseProcessorConsumerService responseHandlerConsumerService,
            IInvoiceSubmissionLogService invoiceSubmissionLogService,
            AppDbContext context
            )
        {
            _requestFactoryConsumerService = requestBuilderConsumerService;
            _httpRequestSenderConsumerService = httpClientConsumerService;
            _responseProcessorConsumerService = responseHandlerConsumerService;
            _invoiceSubmissionLogService = invoiceSubmissionLogService;
            _context = context;
        }
        public async Task<GetRecentDocumentsResponseDTO> GetRecentDocuments()
        {
            var request = _requestFactoryConsumerService.GetRecentDocuments();
            var response = await _httpRequestSenderConsumerService.ExecuteWithAuthRetryAsync(request);
            return await _responseProcessorConsumerService.GetRecentDocuments(response);
        }

        public async Task<SubmitDocumentsResponseDTO> SubmitDocuments(List<ProviderInvoiceViewModel> providerInvoices)
        {
            var request = await _requestFactoryConsumerService.SubmitDocuments(providerInvoices);
            var response = await _httpRequestSenderConsumerService.ExecuteWithAuthRetryAsync(request);
            var processedResponse = await _responseProcessorConsumerService.SubmitDocuments(response);

            if (processedResponse.IsSuccess)
                await _invoiceSubmissionLogService.LogInvoiceSubmission(processedResponse.SuccessfulResponseDTO);

            return processedResponse;
        }
    }
}
