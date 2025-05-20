using ETA.Integrator.Server.Models.Provider;
using RestSharp;

namespace ETA.Integrator.Server.Interface.Services
{
    public interface IConsumerService
    {
        Task<RestRequest> SubmitInvoiceRequest(List<ProviderInvoiceViewModel> invoicesList);
        RestRequest GetRecentDocumentsRequest();
    }
}
