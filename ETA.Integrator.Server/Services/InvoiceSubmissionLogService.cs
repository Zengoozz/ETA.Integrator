using ETA.Integrator.Server.Dtos.ConsumerAPI.Submission;
using ETA.Integrator.Server.Dtos.ConsumerAPI.SubmitDocuments;
using ETA.Integrator.Server.Entities;
using ETA.Integrator.Server.Helpers;
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

        public async Task<SubmitDocumentsResponseDTO> LogInvoiceSubmission(SuccessfulResponseDTO responseDTO, List<SubmissionSummaryDTO> submissions)
        {
            string responseMessage = "";
            List<InvoiceSubmissionLog> invoiceSubmissionLogs = new List<InvoiceSubmissionLog>();
            var utcNow = GenericHelpers.GetCurrentUTCTime(-70);


            var listOfAccepted = responseDTO.AcceptedDocuments.Select(x => new InvoiceSubmissionLog
            {
                InternalId = x.InternalId,
                SubmissionId = x.Uuid,
                Status = (InvoiceStatus)Enum.Parse(typeof(InvoiceStatus), submissions.FirstOrDefault(d => d.InternalId == x.InternalId)?.Status ?? "Submitted"),
                StatusStringfied = submissions.FirstOrDefault(d => d.InternalId == x.InternalId)?.Status ?? "Submitted",
                SubmissionDate = submissions.FirstOrDefault(d => d.InternalId == x.InternalId)?.DateTimeIssued ?? utcNow,
            });

            responseMessage += !listOfAccepted.Any() ? $"Submitted: NONE\n"
                : $"Submitted: {string.Join(" / ", listOfAccepted.Select(n => $"#{n.InternalId}"))}\n";

            invoiceSubmissionLogs.AddRange(listOfAccepted);

            var listOfRejected = responseDTO.RejectedDocuments.Select(x => new InvoiceSubmissionLog
            {
                InternalId = x.InternalId,
                Status = InvoiceStatus.Rejected,
                StatusStringfied = "Rejected",
                SubmissionDate = utcNow,
                RejectionReasonJSON = x.Error is not null ? JsonSerializer.Serialize(x.Error) : ""
            });

            responseMessage += !listOfRejected.Any() ? $"Rejected: NONE\n"
                : $"Rejected: {string.Join(" / ", listOfRejected.Select(n => $"#{n.InternalId}"))}\n";

            invoiceSubmissionLogs.AddRange(listOfRejected);
            
            await _invoiceSubmissionLogRepository.SaveList(invoiceSubmissionLogs);

            SubmitDocumentsResponseDTO response = new SubmitDocumentsResponseDTO()
            {
                IsAllSuccess = !listOfRejected.Any(),
                IsAllFailure = !listOfAccepted.Any(),
                ResponseMessage = responseMessage
            };

            return response;
        }

        public async Task ValidateInvoiceStatus(List<ProviderInvoiceViewModel> invoices)
        {
            var listOfInvoiceIds = invoices.Select(x => x.InvoiceId.ToString()).ToList();
            var invoiceLogs = await _invoiceSubmissionLogRepository.GetByListOfInternalIds(listOfInvoiceIds);
            
            if(invoiceLogs.Count > 0)
            {
                foreach (var invoice in invoices)
                {
                    invoice.IsReviewed = invoiceLogs.Any(x => x.InternalId == invoice.InvoiceId.ToString() && x.Status >= InvoiceStatus.Submitted);
                }
            }
        }
    }
}
