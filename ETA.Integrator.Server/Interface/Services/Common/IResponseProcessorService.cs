using ETA.Integrator.Server.Dtos.ConsumerAPI.GetRecentDocuments;
using ETA.Integrator.Server.Dtos.ConsumerAPI.SubmitDocuments;
using ETA.Integrator.Server.Models.Consumer.Response;
using ETA.Integrator.Server.Models.Provider;
using ETA.Integrator.Server.Models.Provider.Response;
using RestSharp;

namespace ETA.Integrator.Server.Interface.Services.Common
{
    public interface IResponseProcessorService
    {
        Task<T> ProcessResponse<T>(RestResponse response) where T : new();
    }
}
