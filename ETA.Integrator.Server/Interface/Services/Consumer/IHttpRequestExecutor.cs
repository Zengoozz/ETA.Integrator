using RestSharp;

namespace ETA.Integrator.Server.Interface.Services.Consumer
{
    public interface IHttpRequestExecutor
    {
        Task SubmitInvoices(RestRequest request);
        Task GetRecentDocuments(RestRequest request);
    }
}
