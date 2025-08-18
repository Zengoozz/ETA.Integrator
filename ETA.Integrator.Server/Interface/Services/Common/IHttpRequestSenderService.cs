using RestSharp;

namespace ETA.Integrator.Server.Interface.Services.Common
{
    public interface IHttpRequestSenderService
    {
        Task<RestResponse> ExecuteWithAuthRetryAsync(RestRequest request);
    }
}
