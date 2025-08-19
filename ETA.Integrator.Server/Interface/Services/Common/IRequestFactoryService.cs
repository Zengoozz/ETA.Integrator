using ETA.Integrator.Server.Dtos;
using ETA.Integrator.Server.Models;
using ETA.Integrator.Server.Models.Provider;
using ETA.Integrator.Server.Models.Provider.Requests;
using RestSharp;

namespace ETA.Integrator.Server.Interface.Services.Common
{
    public interface IRequestFactoryService
    {
        RestRequest ConnectToProvider(ProviderLoginRequestModel model);
        Task<RestRequest> ConnectToConsumer(ConnectionDTO? model = null);
        Task<RestRequest> SubmitDocuments(List<ProviderInvoiceViewModel> invoicesList);
        RestRequest GetRecentDocuments();
        RestRequest GetProviderInvoices(DateTime? fromDate, DateTime? toDate, string invoiceType);
    }
}
