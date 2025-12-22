using ETA.Integrator.Server.Dtos;
using ETA.Integrator.Server.Dtos.ConsumerAPI;
using ETA.Integrator.Server.Dtos.ConsumerAPI.RecentDocuments;
using ETA.Integrator.Server.Dtos.ConsumerAPI.SearchDocuments;
using ETA.Integrator.Server.Dtos.ConsumerAPI.Submission;
using ETA.Integrator.Server.Dtos.ConsumerAPI.SubmitDocuments;
using ETA.Integrator.Server.Entities;
using ETA.Integrator.Server.Helpers.Enums;
using ETA.Integrator.Server.Interface.Services;
using ETA.Integrator.Server.Interface.Services.Common;
using ETA.Integrator.Server.Models;
using ETA.Integrator.Server.Models.Consumer.Response;
using ETA.Integrator.Server.Models.Core;
using ETA.Integrator.Server.Models.Provider;
using ETA.Integrator.Server.Models.Provider.Requests;
using ETA.Integrator.Server.Models.Provider.Response;
using Microsoft.Extensions.Options;
using RestSharp;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Reflection.Metadata;

namespace ETA.Integrator.Server.Services.Common
{
    public class ApiCallerService : IApiCallerService
    {
        private readonly CustomConfigurations _customConfig;
        private readonly IRequestFactoryService _requestFactoryService;
        private readonly IHttpRequestSenderService _httpRequestSenderService;
        private readonly IResponseProcessorService _responseProcessorService;
        private readonly IInvoiceSubmissionLogService _invoiceSubmissionLogService;
        public ApiCallerService(
            IOptions<CustomConfigurations> customConfigurations,
            IRequestFactoryService requestFactoryService,
            IHttpRequestSenderService httpRequestSenderService,
            IResponseProcessorService responseProcessorService,
            IInvoiceSubmissionLogService invoiceSubmissionLogService
            )
        {
            _customConfig = customConfigurations.Value;
            _requestFactoryService = requestFactoryService;
            _httpRequestSenderService = httpRequestSenderService;
            _responseProcessorService = responseProcessorService;
            _invoiceSubmissionLogService = invoiceSubmissionLogService;
        }

        public async Task<ProviderLoginResponseModel> ConnectToProvider(ProviderLoginRequestModel model)
        {

            if (_customConfig.Provider_APIURL is null)
                throw new ProblemDetailsException(
                    StatusCodes.Status500InternalServerError,
                    "Provider_APIURL not found",
                    "Getting provider api url failed"
                    );

            GenericRequest request = _requestFactoryService.ConnectToProvider(model);
            RestResponse response = await _httpRequestSenderService.SendRequest(request);
            ProviderLoginResponseModel processedResponse = await _responseProcessorService.ProcessResponse<ProviderLoginResponseModel>(response);

            _customConfig.Provider_Token = processedResponse.Token ?? "";

            return processedResponse;
        }

        public async Task<ConsumerConnectionResponseModel> ConnectToConsumer(ConnectionDTO? model)
        {
            GenericRequest request = await _requestFactoryService.ConnectToConsumer(model);
            RestResponse response = await _httpRequestSenderService.SendRequest(request);
            ConsumerConnectionResponseModel processedResponse = await _responseProcessorService.ProcessResponse<ConsumerConnectionResponseModel>(response);

            if (string.IsNullOrEmpty(processedResponse.access_token))
                throw new ProblemDetailsException(
                    StatusCodes.Status401Unauthorized,
                    "AUTH_FAILED",
                    "ETA token did not get extracted correctly"
                    );

            _customConfig.Consumer_Token = processedResponse.access_token;

            return processedResponse;
        }

        public async Task<List<ProviderInvoiceViewModel>> GetProviderInvoices(ProviderInvoicesSearchDTO searchModel)
        {
            GenericRequest request = _requestFactoryService.GetProviderInvoices(searchModel);
            RestResponse response = await _httpRequestSenderService.SendRequest(request);
            List<ProviderInvoiceViewModel> processedResponse = await _responseProcessorService.ProcessResponse<List<ProviderInvoiceViewModel>>(response);

            if (processedResponse.Count() > 0)
                await _invoiceSubmissionLogService.ValidateInvoiceStatus(processedResponse);

            return processedResponse;
        }

        public async Task<List<ProviderInvoiceViewModel>> GetProviderNotes(ProviderInvoicesSearchDTO searchModel)
        {
            GenericRequest request = _requestFactoryService.GetProviderNotes(searchModel);
            RestResponse response = await _httpRequestSenderService.SendRequest(request);
            List<ProviderInvoiceViewModel> processedResponse = await _responseProcessorService.ProcessResponse<List<ProviderInvoiceViewModel>>(response);
            if (processedResponse.Count() > 0)
                await _invoiceSubmissionLogService.ValidateInvoiceStatus(processedResponse);
            return processedResponse;
        }

        public async Task<RecentDocumentsResponseDTO> GetRecentDocuments()
        {
            GenericRequest request = _requestFactoryService.GetRecentDocuments();
            RestResponse response = await _httpRequestSenderService.SendRequest(request);
            return await _responseProcessorService.ProcessResponse<RecentDocumentsResponseDTO>(response);
        }

        public async Task<SubmitDocumentsResponseDTO> SubmitInvoices(InvoiceRequest invoicesRequest)
        {
            if (invoicesRequest.ForNotes)
            {
                invoicesRequest.InternalIdsWithUUIDs = await ValidateReferences(invoicesRequest.Invoices);
            }

            GenericRequest request = await _requestFactoryService.SubmitInvoices(invoicesRequest);
            RestResponse response = await _httpRequestSenderService.SendRequest(request);
            SuccessfulResponseDTO processedResponse = await _responseProcessorService.ProcessResponse<SuccessfulResponseDTO>(response);

            SubmissionResponseDTO submissionResponse = new();
            SubmitDocumentsResponseDTO logResponse = await _invoiceSubmissionLogService.LogInvoiceSubmission(processedResponse, invoicesRequest.Invoices);

            if (!String.IsNullOrEmpty(processedResponse.SubmissionId))
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                submissionResponse = await GetSubmission(processedResponse.SubmissionId, 1, invoicesRequest.Invoices.Count);

                if (submissionResponse.DocumentSummary.Count > 0)
                    await _invoiceSubmissionLogService.UpdateWithSubmissionStatus(processedResponse.AcceptedDocuments, submissionResponse.DocumentSummary);
            }

            return logResponse;
        }

        public async Task<SubmitDocumentsResponseDTO> ResubmitInvoices(InvoiceRequest invoicesRequest)
        {
            List<InvoiceSubmissionLog> logs = await _invoiceSubmissionLogService.GetAllValidWithIds(invoicesRequest.InvoicesIds);
            List<string> internalIds = logs.Select(l => l.InternalId).ToList();

            if (logs.Count > 0)
                return new SubmitDocumentsResponseDTO()
                {
                    IsError = true,
                    ResponseMessage = $"The following invoices are already submitted: {string.Join(" / ", internalIds)}.\n Uncheck all of those submitted ones and try again."
                };

            ProviderInvoicesSearchDTO searchModel = new ProviderInvoicesSearchDTO()
            {
                StartDate = null,
                EndDate = null,
                InvoiceType = invoicesRequest.InvoiceType,
                InvoicesIds = invoicesRequest.InvoicesIds
            };

            invoicesRequest.Invoices = await GetProviderInvoices(searchModel);

            if (invoicesRequest.Invoices.Count == 0)
                return new SubmitDocumentsResponseDTO()
                {
                    IsError = true,
                    ResponseMessage = $"No Invoices with the following codes: {string.Join(" / ", invoicesRequest.InvoicesIds)} was found."
                };

            return await SubmitInvoices(invoicesRequest);
        }

        public async Task<SubmissionResponseDTO> GetSubmission(string submissionId, int pageNo = 1, int pageSize = 100)
        {
            SubmissionResponseDTO processedResponse = new SubmissionResponseDTO();
            processedResponse.OverallStatus = "InProgress";

            pageSize = pageSize > 100 ? pageSize : 100;
            GenericRequest request = _requestFactoryService.GetSubmission(submissionId, pageNo, pageSize);
            RestResponse restResponse = await _httpRequestSenderService.SendRequest(request);

            if (restResponse.StatusCode != HttpStatusCode.NotFound)
            {
                processedResponse = await _responseProcessorService.ProcessResponse<SubmissionResponseDTO>(restResponse);
            }

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            while (stopWatch.Elapsed < TimeSpan.FromMinutes(1) && (restResponse.StatusCode == HttpStatusCode.NotFound || processedResponse.OverallStatus == "InProgress"))
            {
                await Task.Delay(TimeSpan.FromSeconds(10));
                restResponse = await _httpRequestSenderService.SendRequest(request);

                if (restResponse.StatusCode != HttpStatusCode.NotFound)
                    processedResponse = await _responseProcessorService.ProcessResponse<SubmissionResponseDTO>(restResponse);
            }
            stopWatch.Stop();

            if (restResponse.StatusCode == HttpStatusCode.NotFound)
            {
                processedResponse.Uuid = submissionId;
            }

            return processedResponse;
        }

        public async Task<SearchDocumentsResponseDTO> SearchDocuments(DateTime submissionDateFrom, DateTime submissionDateTo, string status, string receiverType, string direction)
        {
            GenericRequest request = _requestFactoryService.SearchDocuments(submissionDateFrom, submissionDateTo, status, receiverType, direction);
            RestResponse response = await _httpRequestSenderService.SendRequest(request);
            return await _responseProcessorService.ProcessResponse<SearchDocumentsResponseDTO>(response);
        }

        public async Task<DocumentExtendedDTO> GetDocument(string uuid)
        {
            GenericRequest request = _requestFactoryService.GetDocument(uuid);
            RestResponse response = await _httpRequestSenderService.SendRequest(request);
            return await _responseProcessorService.ProcessResponse<DocumentExtendedDTO>(response);
        }

        public async Task RevalidateSubmission(string internalId)
        {
            List<InvoiceSubmissionLog> logs = await _invoiceSubmissionLogService.GetForOnlySubmittedByInternalIdDescOrdered(internalId);
            List<int> alreadyUpdatedLogs = new();

            if (logs.Count <= 0)
                return;

            foreach (InvoiceSubmissionLog log in logs)
            {
                DocumentExtendedDTO documentResponse = await GetDocument(log.Uuid);

                if (documentResponse.Status == "Valid")
                {
                    await _invoiceSubmissionLogService.UpdateStatus(log.Id, InvoiceStatus.Valid);

                    List<int> invalidLogsIds = logs.Where(l => l.Id != log.Id && !alreadyUpdatedLogs.Contains(l.Id)).Select(l => l.Id).ToList();

                    if (invalidLogsIds.Count > 0)
                        await _invoiceSubmissionLogService.UpdateStatusWithListOfIds(invalidLogsIds, InvoiceStatus.Invalid);

                    break;
                }

                await _invoiceSubmissionLogService.UpdateStatus(log.Id, (InvoiceStatus)Enum.Parse(typeof(InvoiceStatus), documentResponse.Status, true));
                alreadyUpdatedLogs.Add(log.Id);
            }
        }

        private async Task<List<KeyValuePair<string, string>>> ValidateReferences(List<ProviderInvoiceViewModel> notes)
        {
            List<string> references = notes.Where(i => !string.IsNullOrEmpty(i.ReferenceId)).Select(i => i.ReferenceId!).ToList();

            if (references is null || references.Count <= 0)
                throw new ProblemDetailsException(
                    StatusCodes.Status400BadRequest,
                    "NO_REFS",
                    "No references found with the notes"
                    );

            if (references.Count != notes.Count)
            {
                List<string> withoutRefs = notes.Where(i => string.IsNullOrEmpty(i.ReferenceId)).Select(i => i.InvoiceNumber).ToList();
                string errMessage = $"The following notes got no references for previous submitted invoices: {string.Join(" / ", withoutRefs)}";

                throw new ProblemDetailsException(
                    StatusCodes.Status400BadRequest,
                    "INVALID_NOTES",
                    errMessage
                    );
            }

            List<KeyValuePair<string, string>> validReferences = new();

            (List<InvoiceSubmissionLog> submitted, List<InvoiceSubmissionLog> valid) = await _invoiceSubmissionLogService.GetValidAndSubmittedByInternalId(references);

            validReferences.AddRange(valid.Select(v => new KeyValuePair<string, string>(v.InternalId, v.Uuid)));

            if (submitted.Count > 0)
            {
                foreach (var record in submitted)
                {
                    await Task.Delay(TimeSpan.FromSeconds(3));

                    DocumentExtendedDTO document = await GetDocument(record.Uuid);

                    await _invoiceSubmissionLogService.UpdateStatus(record.Id, (InvoiceStatus)Enum.Parse(typeof(InvoiceStatus), document.Status, true));

                    if (document != null && document.Status.ToLower() == "valid")
                    {
                        validReferences.Add(new KeyValuePair<string, string>(record.InternalId, record.Uuid));
                    }
                }
            }

            if (validReferences.Count != references.Count)
            {
                var invalidRefs = references.Except(validReferences.Select(v => v.Key)).ToList();
                var errMessage = $"The following references are invalid or not submitted yet: {string.Join(" / ", invalidRefs)}";

                throw new ProblemDetailsException(
                    StatusCodes.Status400BadRequest,
                    "INVALID_REFS",
                    errMessage
                    );
            }

            return validReferences;

        }
    }
}
