using ETA.Integrator.Server.Dtos;
using ETA.Integrator.Server.Dtos.ConsumerAPI.RecentDocuments;
using ETA.Integrator.Server.Dtos.ConsumerAPI.Submission;
using ETA.Integrator.Server.Dtos.ConsumerAPI.SearchDocuments;
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
using System.Net;

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

        public async Task<RecentDocumentsResponseDTO> GetRecentDocuments()
        {
            GenericRequest request = _requestFactoryService.GetRecentDocuments();
            RestResponse response = await _httpRequestSenderService.SendRequest(request);
            return await _responseProcessorService.ProcessResponse<RecentDocumentsResponseDTO>(response);
        }

        public async Task<SubmitDocumentsResponseDTO> SubmitDocuments(InvoiceRequest invoicesRequest)
        {
            GenericRequest request = await _requestFactoryService.SubmitDocuments(invoicesRequest);
            RestResponse response = await _httpRequestSenderService.SendRequest(request);
            SuccessfulResponseDTO processedResponse = await _responseProcessorService.ProcessResponse<SuccessfulResponseDTO>(response);

            SubmissionResponseDTO submissionResponse = new();
            if (!String.IsNullOrEmpty(processedResponse.SubmissionId))
                submissionResponse = await GetSubmission(processedResponse.SubmissionId, 1, invoicesRequest.Invoices.Count);

            SubmitDocumentsResponseDTO logResponse = await _invoiceSubmissionLogService.LogInvoiceSubmission(processedResponse, submissionResponse.DocumentSummary, invoicesRequest.Invoices);

            return logResponse;
        }

        public async Task<SubmissionResponseDTO> GetSubmission(string submissionId, int pageNo = 1, int pageSize = 100)
        {
            await Task.Delay(1000);
            pageSize = pageSize > 100 ? pageSize : 100;
            GenericRequest request = _requestFactoryService.GetSubmission(submissionId, pageNo, pageSize);
            RestResponse response = await _httpRequestSenderService.SendRequest(request);
            int retries = 0;
            while (response.StatusCode == HttpStatusCode.NotFound && retries < 3)
            {
                await Task.Delay(1000);
                response = await _httpRequestSenderService.SendRequest(request);
                retries++;
            }
            //case not found also 
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new SubmissionResponseDTO()
                {
                    Uuid = submissionId,
                    DocumentSummary = []
                };
            }
            return await _responseProcessorService.ProcessResponse<SubmissionResponseDTO>(response);
        }

        public async Task<SearchDocumentsResponseDTO> SearchDocuments(DateTime submissionDateFrom, DateTime submissionDateTo, string status, string recieverType, string direction)
        {
            GenericRequest request = _requestFactoryService.SearchDocuments(submissionDateFrom, submissionDateTo, status, recieverType, direction);
            RestResponse response = await _httpRequestSenderService.SendRequest(request);
            return await _responseProcessorService.ProcessResponse<SearchDocumentsResponseDTO>(response);
        }
    }
}
