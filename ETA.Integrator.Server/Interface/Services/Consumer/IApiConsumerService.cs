using ETA.Integrator.Server.Models.Provider;
using RestSharp;

namespace ETA.Integrator.Server.Interface.Services.Consumer
{
    public interface IApiConsumerService
    {
        Task SubmitInvoices(List<ProviderInvoiceViewModel> providerInvoices);
        Task GetRecentDocuments();
    }
}
