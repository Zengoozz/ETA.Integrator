using ETA.Integrator.Server.Dtos.ConsumerAPI.SubmitDocuments;

namespace ETA.Integrator.Server.Interface.Services
{
    public interface IInvoiceSubmissionLogService
    {
        Task LogInvoiceSubmission(SuccessfulResponseDTO responseDTO);
    }
}
