using ETA.Integrator.Server.Models.Consumer.ETA;
using ETA.Integrator.Server.Models.Provider;
using RestSharp;

namespace ETA.Integrator.Server.Interface.Services
{
    public interface IConsumerService
    {
        Task<RestRequest> SubmitInvoiceRequest(List<ProviderInvoiceViewModel> invoicesList);
        InvoiceModel PrepareInvoiceDetails(ProviderInvoiceViewModel invoiceViewModel, IssuerModel issuer, string tokenPin);
        RestRequest GetRecentDocumentsRequest();
    }
}
