using ETA.Integrator.Server.Dtos.ConsumerAPI.GetRecentDocuments;
using ETA.Integrator.Server.Dtos.ConsumerAPI.SubmitDocuments;
using ETA.Integrator.Server.Models.Provider;
using RestSharp;

namespace ETA.Integrator.Server.Interface.Services.Common
{
    public interface IApiCallerService
    {
        Task<SubmitDocumentsResponseDTO> SubmitDocuments(List<ProviderInvoiceViewModel> providerInvoices);
        Task<GetRecentDocumentsResponseDTO> GetRecentDocuments();
        Task<List<ProviderInvoiceViewModel>> GetProviderInvoices(DateTime? fromDate, DateTime? toDate, string invoiceType);

    }
}
