using ETA.Integrator.Server.Interface.Services.Consumer;
using RestSharp;

namespace ETA.Integrator.Server.Services.Consumer
{
    public class ResponseProcessorConsumerService : IResponseProcessorConsumerService
    {
        public Task GetRecentDocuments(RestResponse response)
        {
            throw new NotImplementedException();
        }

        public Task SubmitInvoices(RestResponse response)
        {
            throw new NotImplementedException();
        }
    }
}
