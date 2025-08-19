using ETA.Integrator.Server.Models.Core;
using RestSharp;

namespace ETA.Integrator.Server.Interface.Services.Common
{
    public interface IHttpRequestSenderService
    {
        Task<RestResponse> SendRequest(GenericRequest request);
    }
}
