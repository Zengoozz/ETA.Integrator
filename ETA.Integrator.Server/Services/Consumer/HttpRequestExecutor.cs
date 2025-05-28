using ETA.Integrator.Server.Interface.Services;
using ETA.Integrator.Server.Interface.Services.Consumer;
using RestSharp;

namespace ETA.Integrator.Server.Services.Consumer
{
    public class HttpRequestExecutor : IHttpRequestExecutor
    {
        private readonly IRequestHandlerService _requestHandlerService;
        public HttpRequestExecutor(IRequestHandlerService requestHandlerService)
        {
            _requestHandlerService = requestHandlerService;
        }
        public async Task GetRecentDocuments(RestRequest request) // Gets Called form controller (takes the controllers parameter directly)
        {
            //TODO: Req Builder Service (Build request) e.g: ConsumerService
            //TODO: Http Client Service (Sends request) e.g: RequestHandlerService
            //TODO: Response Handler (Validate response & Do bussiness logic for it like logging) NOT_IMPLEMENTED_YET
            var response = await _requestHandlerService.ExecuteWithAuthRetryAsync(request);
            throw new NotImplementedException();
        }

        public async Task SubmitInvoices(RestRequest request)
        {
            var response = await _requestHandlerService.ExecuteWithAuthRetryAsync(request);

            throw new NotImplementedException();
        }
    }
}
