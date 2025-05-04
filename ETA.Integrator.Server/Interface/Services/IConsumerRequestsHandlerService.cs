using ETA.Integrator.Server.Models.Consumer.Response;
using RestSharp;

namespace ETA.Integrator.Server.Interface.Services
{
    public interface IConsumerRequestsHandlerService
    {
        Task<GenericResponse<string>> AuthorizeConsumer();
        Task<GenericResponse<RestResponse?>> ExecuteWithAuthRetryAsync(RestRequest request);
    }
}
