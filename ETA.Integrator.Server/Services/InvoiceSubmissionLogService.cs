using ETA.Integrator.Server.Dtos;
using ETA.Integrator.Server.Dtos.ConsumerAPI.Submission;
using ETA.Integrator.Server.Dtos.ConsumerAPI.SubmitDocuments;
using ETA.Integrator.Server.Entities;
using ETA.Integrator.Server.Helpers;
using ETA.Integrator.Server.Helpers.Enums;
using ETA.Integrator.Server.Interface.Repositories;
using ETA.Integrator.Server.Interface.Services;
using ETA.Integrator.Server.Models.Provider;
using System.Linq;
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

        public async Task<List<InvoiceSubmissionLog>> GetAll()
        {
            return await _invoiceSubmissionLogRepository.GetAll();
        }

        public async Task<List<InvoiceSubmissionLog>> GetAllValidWithIds(List<string> invoicesIds)
        {
            return await _invoiceSubmissionLogRepository.GetAllValidWithIds(invoicesIds);
        }

        public async Task<List<InvoiceSubmissionLog>> GetUnvalidatedSubmissions()
        {
            return await _invoiceSubmissionLogRepository.GetUnvalidatedSubmissions();
        }

        public async Task SaveList(List<InvoiceSubmissionLog> listOfEntities)
        {
            await _invoiceSubmissionLogRepository.SaveList(listOfEntities);
        }

        public async Task<SubmitDocumentsResponseDTO> LogInvoiceSubmission(SuccessfulResponseDTO submitResponseDTO, List<ProviderInvoiceViewModel> invoices)
        {
            string responseMessage = "";
            List<InvoiceSubmissionLog> invoiceSubmissionLogs = new List<InvoiceSubmissionLog>();
            var utcNow = GenericHelpers.GetCurrentUTCTime(-70);

            var listOfAccepted = submitResponseDTO.AcceptedDocuments.Select(x => new InvoiceSubmissionLog
            {
                InternalId = x.InternalId,
                Uuid = x.Uuid,
                SubmissionId = submitResponseDTO.SubmissionId,
                Status = (InvoiceStatus)Enum.Parse(typeof(InvoiceStatus), "Submitted"),
                StatusStringfied = "Submitted",
                SubmissionDate = utcNow,
            });

            responseMessage += !listOfAccepted.Any() ?
                $"Submitted: NONE\n" :
                $"Submitted: {string.Join(" / ",
                    invoices.Where(i => listOfAccepted.Select(n => n.InternalId).Contains(i.InvoiceId)).Select(i => $"#{i.InvoiceNumber}"))}\n";

            invoiceSubmissionLogs.AddRange(listOfAccepted);

            var listOfRejected = submitResponseDTO.RejectedDocuments.Select(x => new InvoiceSubmissionLog
            {
                InternalId = x.InternalId,
                Status = InvoiceStatus.Rejected,
                StatusStringfied = "Rejected",
                SubmissionDate = utcNow,
                RejectionReasonJSON = x.Error is not null ? JsonSerializer.Serialize(x.Error) : ""
            });

            responseMessage += !listOfRejected.Any() ?
                $"Rejected: NONE\n" :
                $"Rejected: {string.Join(" / ",
                    invoices.Where(i => listOfRejected.Select(n => n.InternalId).Contains(i.InvoiceId)).Select(i => $"#{i.InvoiceNumber}"))}\n";

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

        public async Task UpdateWithSubmissionStatus(List<DocumentAcceptedDTO> acceptedDocuments, List<SubmissionSummaryDTO> submissionStatusList)
        {
            List<string> uuids = acceptedDocuments.Select(s => s.Uuid).ToList();
            var alreadyLogged = await _invoiceSubmissionLogRepository.GetByListOfUuids(uuids);

            var submissionToUpdate = alreadyLogged.Select(l => new UpdateSubmissionStatusDTO()
            {
                LoggedId = l.Id,
                SubmissionTime = submissionStatusList.FirstOrDefault(s => s.InternalId == l.InternalId)?.DateTimeIssued ?? null,
                SubmissionsStatus = (InvoiceStatus)Enum.Parse(typeof(InvoiceStatus), submissionStatusList.FirstOrDefault(s => s.InternalId == l.InternalId)?.Status ?? "Submitted")
            });

            await _invoiceSubmissionLogRepository.UpdateListOfSubmissionsStatus(submissionToUpdate.ToList());
        }

        public async Task ValidateInvoiceStatus(List<ProviderInvoiceViewModel> invoices)
        {
            var listOfInvoiceIds = invoices.Select(x => x.InvoiceId).ToList();
            var invoiceLogs = await _invoiceSubmissionLogRepository.GetByListOfInternalIds(listOfInvoiceIds);

            if (invoiceLogs.Count > 0)
            {
                foreach (var invoice in invoices)
                {
                    var invoiceLog = invoiceLogs.FirstOrDefault(x => x.InternalId == invoice.InvoiceId);

                    if (invoiceLog is not null)
                    {
                        invoice.IsReviewed = invoiceLog.Status >= InvoiceStatus.Submitted;
                        invoice.ReviewStatus = invoiceLog.StatusStringfied;
                    }
                }
            }
        }

        public async Task<InvoiceSubmissionLog?> GetValidByInternalId(string internalId)
        {
            return await _invoiceSubmissionLogRepository.GetValidByInternalId(internalId);
        }

        public async Task<(List<InvoiceSubmissionLog> submitted, List<InvoiceSubmissionLog> valid)> GetValidAndSubmittedByInternalId(List<string> internalIds)
        {
            return await _invoiceSubmissionLogRepository.GetValidAndSubmittedByInternalId(internalIds);
        }

        public async Task UpdateStatus(int id, InvoiceStatus status)
        {
            await _invoiceSubmissionLogRepository.UpdateStatus(id, status);
        }
    }
}
