using RestSharp;

namespace ETA.Integrator.Server.Interface.Services
{
    public interface IRequestHandlerService
    {
        Task<string> AuthorizeConsumer();
        Task<RestResponse> ExecuteWithAuthRetryAsync(RestRequest request);
    }
}
