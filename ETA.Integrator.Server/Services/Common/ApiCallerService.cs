using ETA.Integrator.Server.Data;
using ETA.Integrator.Server.Dtos.ConsumerAPI.GetRecentDocuments;
using ETA.Integrator.Server.Dtos.ConsumerAPI.SubmitDocuments;
using ETA.Integrator.Server.Interface.Services;
using ETA.Integrator.Server.Interface.Services.Common;
using ETA.Integrator.Server.Models.Core;
using ETA.Integrator.Server.Models.Provider;
using ETA.Integrator.Server.Models.Provider.Requests;
using ETA.Integrator.Server.Models.Provider.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RestSharp;
using System.Transactions;

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
                    "ApiCallerService/ConnectToProvider: Provider_APIURL not found",
                    "Getting provider api url failed"
                    );

            var opt = new RestClientOptions(_customConfig.Provider_APIURL);
            var connectionClient = new RestClient(opt);

            var request = _requestFactoryService.ConnectToProvider(model);
            var response = await connectionClient.ExecuteAsync(request);
            var processedResponse = await _responseProcessorService.ConnectToProvider(response);

            _customConfig.Provider_Token = processedResponse.Token ?? "";
            
            return processedResponse;
        }

        public async Task<List<ProviderInvoiceViewModel>> GetProviderInvoices(DateTime? fromDate, DateTime? toDate, string invoiceType)
        {
            var request = _requestFactoryService.GetProviderInvoices(fromDate, toDate, invoiceType);

            var client = new RestClient();
            if (!String.IsNullOrWhiteSpace(_customConfig.Provider_APIURL))
            {
                var opt = new RestClientOptions(_customConfig.Provider_APIURL);
                client = new RestClient(opt);
            }
            else
                throw new ProblemDetailsException(
                    StatusCodes.Status500InternalServerError,
                    "ApiCallerService/GetProviderInvoices: CONFIG_NOT_FOUND",
                    "Getting provider API_URL Error"
                    );

            var response = await client.ExecuteAsync(request);
            var processedResponse = await _responseProcessorService.GetProviderInvoices(response);

            if (processedResponse.Count() > 0)
                await _invoiceSubmissionLogService.ValidateInvoiceStatus(processedResponse);

            return processedResponse;
        }

        public async Task<GetRecentDocumentsResponseDTO> GetRecentDocuments()
        {
            var request = _requestFactoryService.GetRecentDocuments();
            var response = await _httpRequestSenderService.ExecuteWithAuthRetryAsync(request);
            return await _responseProcessorService.GetRecentDocuments(response);
        }

        public async Task<SubmitDocumentsResponseDTO> SubmitDocuments(List<ProviderInvoiceViewModel> providerInvoices)
        {
            var request = await _requestFactoryService.SubmitDocuments(providerInvoices);
            var response = await _httpRequestSenderService.ExecuteWithAuthRetryAsync(request);
            var processedResponse = await _responseProcessorService.SubmitDocuments(response);

            if (processedResponse.IsSuccess)
                await _invoiceSubmissionLogService.LogInvoiceSubmission(processedResponse.SuccessfulResponseDTO);

            return processedResponse;
        }
    }
}
