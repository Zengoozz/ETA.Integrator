using ETA.Integrator.Server.Dtos.ConsumerAPI;
using ETA.Integrator.Server.Dtos.ConsumerAPI.SubmitDocuments;
using ETA.Integrator.Server.Models.Provider;

namespace ETA.Integrator.Server.Interface.Services
{
    public interface IInvoiceSubmissionLogService
    {
        Task<SubmitDocumentsResponseDTO> LogInvoiceSubmission(SuccessfulResponseDTO responseDTO, List<SubmissionSummaryDTO> submissions);
        Task ValidateInvoiceStatus(List<ProviderInvoiceViewModel> invoices);
    }
}
