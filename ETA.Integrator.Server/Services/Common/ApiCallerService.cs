﻿using ETA.Integrator.Server.Dtos;
using ETA.Integrator.Server.Dtos.ConsumerAPI.GetRecentDocuments;
using ETA.Integrator.Server.Dtos.ConsumerAPI.GetSubmission;
using ETA.Integrator.Server.Dtos.ConsumerAPI.SubmitDocuments;
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

        public async Task<List<ProviderInvoiceViewModel>> GetProviderInvoices(DateTime? fromDate, DateTime? toDate, string invoiceType)
        {
            GenericRequest request = _requestFactoryService.GetProviderInvoices(fromDate, toDate, invoiceType);
            RestResponse response = await _httpRequestSenderService.SendRequest(request);
            List<ProviderInvoiceViewModel> processedResponse = await _responseProcessorService.ProcessResponse<List<ProviderInvoiceViewModel>>(response);

            if (processedResponse.Count() > 0)
                await _invoiceSubmissionLogService.ValidateInvoiceStatus(processedResponse);

            return processedResponse;
        }

        public async Task<GetRecentDocumentsResponseDTO> GetRecentDocuments()
        {
            GenericRequest request = _requestFactoryService.GetRecentDocuments();
            RestResponse response = await _httpRequestSenderService.SendRequest(request);
            return await _responseProcessorService.ProcessResponse<GetRecentDocumentsResponseDTO>(response);
        }

        public async Task<SubmitDocumentsResponseDTO> SubmitDocuments(InvoiceRequest invoices)
        {
            List<ProviderInvoiceViewModel> providerInvoices = invoices.Invoices;
            GenericRequest request = await _requestFactoryService.SubmitDocuments(invoices);
            RestResponse response = await _httpRequestSenderService.SendRequest(request);
            SuccessfulResponseDTO processedResponse = await _responseProcessorService.ProcessResponse<SuccessfulResponseDTO>(response);
            await _invoiceSubmissionLogService.LogInvoiceSubmission(processedResponse);

            List<string> acceptedInvoicesIds = processedResponse.AcceptedDocuments.Select(a => a.InternalId).ToList();
            List<string> acceptedInvoicesNumbers = providerInvoices.Where(i => acceptedInvoicesIds.Contains(i.InvoiceId.ToString())).Select(i => i.InvoiceNumber).ToList();
            List<string> rejectedInvoicesNumbers = providerInvoices.Where(i => !acceptedInvoicesIds.Contains(i.InvoiceId.ToString())).Select(i => i.InvoiceNumber).ToList();

            string responseMsg = acceptedInvoicesNumbers.Count == 0 ? $"Submitted: NONE\n"
                : $"Submitted: {string.Join(" / ", acceptedInvoicesNumbers.Select(n => $"#{n}"))}\n";

            responseMsg += rejectedInvoicesNumbers.Count == 0 ? $"Rejected: NONE\n"
                : $"Rejected: {string.Join(" / ", rejectedInvoicesNumbers.Select(n => $"#{n}"))}";

            SubmitDocumentsResponseDTO finalResponse = new SubmitDocumentsResponseDTO()
            {
                IsAllSuccess = rejectedInvoicesNumbers.Count == 0,
                IsAllFailure = acceptedInvoicesNumbers.Count == 0,
                ResponseMessage = responseMsg
            };

            return finalResponse;
        }

        public async Task<List<GetSubmissionResponseDTO>> GetSubmission(string uuid, int pageNumber = 5, int pageSize = 10)
        {
            GenericRequest request = _requestFactoryService.GetSubmission(uuid, pageNumber, pageSize);
            RestResponse response = await _httpRequestSenderService.SendRequest(request);
            return await _responseProcessorService.ProcessResponse<List<GetSubmissionResponseDTO>>(response);
        }

        public async Task<GetRecentDocumentsResponseDTO> SearchDocuments(DateTime submissionDateFrom, DateTime submissionDateTo)
        {
            GenericRequest request = _requestFactoryService.SearchDocuments(submissionDateFrom, submissionDateTo); // Implement when needed
            RestResponse response = await _httpRequestSenderService.SendRequest(request);
            return await _responseProcessorService.ProcessResponse<GetRecentDocumentsResponseDTO>(response);
        }
    }
}
