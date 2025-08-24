using ETA.Integrator.Server.Dtos.ConsumerAPI.SubmitDocuments;
using ETA.Integrator.Server.Models.Provider;

namespace ETA.Integrator.Server.Interface.Services
{
    public interface IInvoiceSubmissionLogService
    {
        Task LogInvoiceSubmission(SuccessfulResponseDTO responseDTO);
        Task ValidateInvoiceStatus(List<ProviderInvoiceViewModel> invoices);
    }
}
