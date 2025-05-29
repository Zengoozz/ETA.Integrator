using ETA.Integrator.Server.Models.Provider;
using RestSharp;

namespace ETA.Integrator.Server.Interface.Services.Consumer
{
    public interface IRequestFactoryConsumerService
    {
        Task<RestRequest> SubmitInvoices(List<ProviderInvoiceViewModel> invoicesList);
        RestRequest GetRecentDocuments();
    }
}
