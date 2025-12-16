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
        Task<List<InvoiceSubmissionLog>> GetUnvalidatedSubmissions();
        Task SaveList(List<InvoiceSubmissionLog> listOfEntities);
        Task<SubmitDocumentsResponseDTO> LogInvoiceSubmission(SuccessfulResponseDTO responseDTO, List<ProviderInvoiceViewModel> invoices);
        Task UpdateWithSubmissionStatus(List<DocumentAcceptedDTO> acceptedDocuments, List<SubmissionSummaryDTO> submissionStatus);
        Task ValidateInvoiceStatus(List<ProviderInvoiceViewModel> invoices);
        Task<InvoiceSubmissionLog?> GetValidByInternalId(string internalId);
    }
}
