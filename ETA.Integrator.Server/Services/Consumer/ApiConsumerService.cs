using ETA.Integrator.Server.Dtos.ConsumerAPI.GetRecentDocuments;
using ETA.Integrator.Server.Interface.Services.Consumer;
using ETA.Integrator.Server.Models.Provider;
using System.Transactions;

namespace ETA.Integrator.Server.Services.Consumer
{
    public class ApiConsumerService : IApiConsumerService
    {
        private readonly IRequestFactoryConsumerService _requestFactoryConsumerService;
        private readonly IHttpRequestSenderConsumerService _httpRequestSenderConsumerService;
        private readonly IResponseProcessorConsumerService _responseProcessorConsumerService;
        public ApiConsumerService(
            IRequestFactoryConsumerService requestBuilderConsumerService,
            IHttpRequestSenderConsumerService httpClientConsumerService,
            IResponseProcessorConsumerService responseHandlerConsumerService
            )
        {
            _requestFactoryConsumerService = requestBuilderConsumerService;
            _httpRequestSenderConsumerService = httpClientConsumerService;
            _responseProcessorConsumerService = responseHandlerConsumerService;
        }
        public async Task<GetRecentDocumentsResponseDTO> GetRecentDocuments()
        {
            var request = _requestFactoryConsumerService.GetRecentDocuments();
            var response = await _httpRequestSenderConsumerService.ExecuteWithAuthRetryAsync(request);
            return await _responseProcessorConsumerService.GetRecentDocuments(response);
        }

        public async Task SubmitInvoices(List<ProviderInvoiceViewModel> providerInvoices)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var request = await _requestFactoryConsumerService.SubmitInvoices(providerInvoices);
                    var response = await _httpRequestSenderConsumerService.ExecuteWithAuthRetryAsync(request);
                    var processedResponse = await _responseProcessorConsumerService.SubmitDocuments(response);

                    scope.Complete();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
