using ETA.Integrator.Server.Dtos.ConsumerAPI.Submission;
using ETA.Integrator.Server.Dtos.ConsumerAPI.SubmitDocuments;
using ETA.Integrator.Server.Entities;
using ETA.Integrator.Server.Models.Provider;

namespace ETA.Integrator.Server.Interface.Services
{
    public interface IInvoiceSubmissionLogService
    {
        Task<List<InvoiceSubmissionLog>> GetAll();
        Task<List<InvoiceSubmissionLog>> GetAllValidWithIds(List<string> invoicesIds);
        Task SaveList(List<InvoiceSubmissionLog> listOfEntities);
        Task<SubmitDocumentsResponseDTO> LogInvoiceSubmission(SuccessfulResponseDTO responseDTO, List<SubmissionSummaryDTO> submissions, List<ProviderInvoiceViewModel> invoices);
        Task ValidateInvoiceStatus(List<ProviderInvoiceViewModel> invoices);
    }
}
