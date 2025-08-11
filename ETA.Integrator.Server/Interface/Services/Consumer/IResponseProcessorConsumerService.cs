using ETA.Integrator.Server.Dtos.ConsumerAPI.GetRecentDocuments;
using ETA.Integrator.Server.Dtos.ConsumerAPI.SubmitDocuments;
using RestSharp;

namespace ETA.Integrator.Server.Interface.Services.Consumer
{
    public interface IResponseProcessorConsumerService
    {
        Task<GetRecentDocumentsResponseDTO> GetRecentDocuments(RestResponse response);
        Task<SubmitDocumentsResponseDTO> SubmitDocuments(RestResponse response);
    }
}
