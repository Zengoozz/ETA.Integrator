using ETA.Integrator.Server.Interface.Services.Consumer;
using ETA.Integrator.Server.Models.Provider;

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
        public async Task GetRecentDocuments()
        {
            var request = _requestFactoryConsumerService.GetRecentDocuments();
            var response = await _httpRequestSenderConsumerService.ExecuteWithAuthRetryAsync(request);
            await _responseProcessorConsumerService.GetRecentDocuments(response);
        }

        public async Task SubmitInvoices(List<ProviderInvoiceViewModel> providerInvoices)
        {
            var request = await _requestFactoryConsumerService.SubmitInvoices(providerInvoices);
            var response = await _httpRequestSenderConsumerService.ExecuteWithAuthRetryAsync(request);
            await _responseProcessorConsumerService.SubmitInvoices(response);
        }
    }
}
