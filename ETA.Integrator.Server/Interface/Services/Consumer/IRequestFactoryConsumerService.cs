using ETA.Integrator.Server.Models.Provider;
using RestSharp;

namespace ETA.Integrator.Server.Interface.Services.Consumer
{
    public interface IRequestFactoryConsumerService
    {
        Task<RestRequest> SubmitDocuments(List<ProviderInvoiceViewModel> invoicesList);
        RestRequest GetRecentDocuments();
    }
}
