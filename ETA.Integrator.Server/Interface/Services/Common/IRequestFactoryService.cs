using ETA.Integrator.Server.Dtos;
using ETA.Integrator.Server.Models;
using ETA.Integrator.Server.Models.Core;
using ETA.Integrator.Server.Models.Provider;
using ETA.Integrator.Server.Models.Provider.Requests;
using RestSharp;

namespace ETA.Integrator.Server.Interface.Services.Common
{
    public interface IRequestFactoryService
    {
        GenericRequest ConnectToProvider(ProviderLoginRequestModel model);
        Task<GenericRequest> ConnectToConsumer(ConnectionDTO? model = null);
        Task<GenericRequest> SubmitDocuments(InvoiceRequest request);
        GenericRequest GetRecentDocuments();
        GenericRequest GetProviderInvoices(DateTime? fromDate, DateTime? toDate, string invoiceType);
        GenericRequest GetSubmission(string uuid, int pageNumber, int pageSize);
        GenericRequest SearchDocuments(DateTime submissionDateFrom, DateTime submissionDateTo);
    }
}
