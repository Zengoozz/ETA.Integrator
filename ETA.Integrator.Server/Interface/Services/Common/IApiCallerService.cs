using ETA.Integrator.Server.Dtos.ConsumerAPI.GetRecentDocuments;
using ETA.Integrator.Server.Dtos.ConsumerAPI.SubmitDocuments;
using ETA.Integrator.Server.Models.Provider;
using ETA.Integrator.Server.Models.Provider.Requests;
using ETA.Integrator.Server.Models.Provider.Response;
using RestSharp;

namespace ETA.Integrator.Server.Interface.Services.Common
{
    public interface IApiCallerService
    {
        Task<ProviderLoginResponseModel> ConnectToProvider(ProviderLoginRequestModel model);
        Task<SubmitDocumentsResponseDTO> SubmitDocuments(List<ProviderInvoiceViewModel> providerInvoices);
        Task<GetRecentDocumentsResponseDTO> GetRecentDocuments();
        Task<List<ProviderInvoiceViewModel>> GetProviderInvoices(DateTime? fromDate, DateTime? toDate, string invoiceType);

    }
}
