using ETA.Integrator.Server.Dtos.ConsumerAPI.GetRecentDocuments;
using ETA.Integrator.Server.Dtos.ConsumerAPI.SubmitDocuments;
using ETA.Integrator.Server.Models.Provider;
using ETA.Integrator.Server.Models.Provider.Response;
using RestSharp;

namespace ETA.Integrator.Server.Interface.Services.Common
{
    public interface IResponseProcessorService
    {
        Task<ProviderLoginResponseModel> ConnectToProvider(RestResponse response);
        Task<List<ProviderInvoiceViewModel>> GetProviderInvoices(RestResponse response);
        Task<GetRecentDocumentsResponseDTO> GetRecentDocuments(RestResponse response);
        Task<SubmitDocumentsResponseDTO> SubmitDocuments(RestResponse response);
    }
}
