using ETA.Integrator.Server.Dtos.ConsumerAPI.GetRecentDocuments;
using RestSharp;

namespace ETA.Integrator.Server.Interface.Services.Consumer
{
    public interface IResponseProcessorConsumerService
    {
        Task SubmitInvoices(RestResponse response);
        Task<ResponseDTO> GetRecentDocuments(RestResponse response);
    }
}
