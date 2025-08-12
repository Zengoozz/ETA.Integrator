using ETA.Integrator.Server.Models.Provider;
using RestSharp;

namespace ETA.Integrator.Server.Interface.Services.Common
{
    public interface IRequestFactoryService
    {
        Task<RestRequest> SubmitDocuments(List<ProviderInvoiceViewModel> invoicesList);
        RestRequest GetRecentDocuments();
        RestRequest GetProviderInvoices(DateTime? fromDate, DateTime? toDate, string invoiceType);
    }
}
