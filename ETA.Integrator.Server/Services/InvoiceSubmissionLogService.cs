using ETA.Integrator.Server.Dtos.ConsumerAPI.SubmitDocuments;
using ETA.Integrator.Server.Entities;
using ETA.Integrator.Server.Helpers.Enums;
using ETA.Integrator.Server.Interface.Repositories;
using ETA.Integrator.Server.Interface.Services;
using ETA.Integrator.Server.Models.Provider;
using System.Text.Json;

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
                SubmissionDate = DateTime.Now,
            });

            invoiceSubmissionLogs.AddRange(listOfAccepted);

            var listOfRejected = responseDTO.RejectedDocuments.Select(x => new InvoiceSubmissionLog
            {
                InternalId = x.InternalId,
                Status = InvoiceStatus.Rejected,
                SubmissionDate = DateTime.Now,
                RejectionReasonJSON = x.Error is not null ? JsonSerializer.Serialize(x.Error) : ""
            });

            invoiceSubmissionLogs.AddRange(listOfRejected);
            
            await _invoiceSubmissionLogRepository.SaveList(invoiceSubmissionLogs);
        }
        public async Task ValidateInvoiceStatus(List<ProviderInvoiceViewModel> invoices)
        {
            var listOfInvoiceIds = invoices.Select(x => x.InvoiceId.ToString()).ToList();
            var invoiceLogs = await _invoiceSubmissionLogRepository.GetByListOfInternalIds(listOfInvoiceIds);
            
            if(invoiceLogs.Count > 0)
            {
                foreach (var invoice in invoices)
                {
                    invoice.IsReviewed = invoiceLogs.Any(x => x.InternalId == invoice.InvoiceId.ToString() && x.Status == InvoiceStatus.Submitted);
                }
            }
        }

    }
}
