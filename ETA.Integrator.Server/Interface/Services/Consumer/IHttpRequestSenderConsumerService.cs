using RestSharp;

namespace ETA.Integrator.Server.Interface.Services.Consumer
{
    public interface IHttpRequestSenderConsumerService
    {
        Task<RestResponse> ExecuteWithAuthRetryAsync(RestRequest request);
    }
}
