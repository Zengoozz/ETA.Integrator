using RestSharp;

namespace ETA.Integrator.Server.Interface.Services.Consumer
{
    public interface IResponseProcessorConsumerService
    {
        Task SubmitInvoices(RestResponse response);
        Task GetRecentDocuments(RestResponse response);
    }
}
