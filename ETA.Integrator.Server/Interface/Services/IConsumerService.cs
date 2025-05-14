using ETA.Integrator.Server.Dtos.InvoiceSubmission;
using ETA.Integrator.Server.Models.Provider;
using RestSharp;

namespace ETA.Integrator.Server.Interface.Services
{
    public interface IConsumerService
    {
        Task<InvoiceSubmissionDTO> SubmitInvoice(List<ProviderInvoiceViewModel> invoices);
        RestRequest GetRecentDocumentsRequest();
    }
}
