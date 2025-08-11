using ETA.Integrator.Server.Dtos.ConsumerAPI.SubmitDocuments;
using ETA.Integrator.Server.Entities;
using ETA.Integrator.Server.Helpers.Enums;
using ETA.Integrator.Server.Interface.Repositories;
using ETA.Integrator.Server.Interface.Services;

namespace ETA.Integrator.Server.Services
{
    public class InvoiceSubmissionLogService : IInvoiceSubmissionLogService
    {
        private readonly IInvoiceSubmissionLogRepository _invoiceSubmissionLogRepository;
        public InvoiceSubmissionLogService(IInvoiceSubmissionLogRepository invoiceSubmissionLogRepository)
        {
            _invoiceSubmissionLogRepository = invoiceSubmissionLogRepository;
        }
        public async Task LogInvoiceSubmission(SuccessfulResponseDTO responseDTO)
        {
            List<InvoiceSubmissionLog> invoiceSubmissionLogs = new List<InvoiceSubmissionLog>();
            
            var listOfAccepted = responseDTO.AcceptedDocuments.Select(x => new InvoiceSubmissionLog
            {
                InternalId = x.InternalId,
                SubmissionId = x.Uuid,
                Status = InvoiceStatus.Submitted,
            });

            invoiceSubmissionLogs.AddRange(listOfAccepted);

            var listOfRejected = responseDTO.RejectedDocuments.Select(x => new InvoiceSubmissionLog
            {
                InternalId = x.InternalId,
                Status = InvoiceStatus.Rejected
            });

            invoiceSubmissionLogs.AddRange(listOfRejected);
            
            await _invoiceSubmissionLogRepository.SaveList(invoiceSubmissionLogs);
        }
    }
}
