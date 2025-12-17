using ETA.Integrator.Server.Dtos;
using ETA.Integrator.Server.Helpers.Enums;
using ETA.Integrator.Server.Interface.Services;
using ETA.Integrator.Server.Interface.Services.Common;
using ETA.Integrator.Server.Models;
using ETA.Integrator.Server.Models.Consumer.ETA;
using ETA.Integrator.Server.Models.Core;
using ETA.Integrator.Server.Models.Provider.Requests;
using Microsoft.Extensions.Options;
using RestSharp;

namespace ETA.Integrator.Server.Services.Common
{
    public class RequestFactoryService : IRequestFactoryService
    {
        private readonly CustomConfigurations _customConfig;
        private readonly ISettingsStepService _settingsStepService;
        private readonly IDocumentSignerService _documentSignerService;
        public RequestFactoryService(
            IOptions<CustomConfigurations> customConfig,
            ISettingsStepService settingsStepService,
            IDocumentSignerService documentSignerService
            )
        {
            _customConfig = customConfig.Value;
            _settingsStepService = settingsStepService ?? throw new ArgumentNullException(nameof(_settingsStepService));
            _documentSignerService = documentSignerService ?? throw new ArgumentNullException(nameof(_settingsStepService));
        }

        public GenericRequest ConnectToProvider(ProviderLoginRequestModel model)
        {
            GenericRequest genericRequest = new();

            genericRequest.Request = new RestRequest("/api/Auth/LogIn", Method.Post).AddJsonBody(model);
            genericRequest.ClientType = ClientType.Provider;

            return genericRequest;
        }

        public async Task<GenericRequest> ConnectToConsumer(ConnectionDTO? model)
        {
            GenericRequest genericRequest = new();
            ConnectionDTO? connectionConfig = model;

            if (connectionConfig is null)
                connectionConfig = await _settingsStepService.GetConnectionData();

            // CLIENT_ID | CLIENT_SECRET VALIDATION
            if (connectionConfig is null || string.IsNullOrWhiteSpace(connectionConfig.ClientId) || string.IsNullOrWhiteSpace(connectionConfig.ClientSecret))
            {
                var errMsg = connectionConfig is null ? "whole" : (string.IsNullOrWhiteSpace(connectionConfig.ClientId) ? "(client_id)" : "(client_secret)");

                throw new ProblemDetailsException(
                    statusCode: StatusCodes.Status400BadRequest,
                    message: "NOT_FOUND",
                    detail: $"Connection configuration {errMsg} not found"
                    );
            }


            genericRequest.Request = new RestRequest("/connect/token", Method.Post)
                .AddParameter("grant_type", "client_credentials")
                .AddParameter("client_id", connectionConfig.ClientId)
                .AddParameter("client_secret", connectionConfig.ClientSecret)
                .AddParameter("scope", "InvoicingAPI");
            genericRequest.ClientType = ClientType.ConsumerAuth;

            return genericRequest;
        }

        public async Task<GenericRequest> SubmitInvoices(InvoiceRequest request)
        {
            GenericRequest genericRequest = new();
            List<string> documents = new List<string>();

            var connectionSettings = await _settingsStepService.GetConnectionData();

            if (connectionSettings is null || string.IsNullOrWhiteSpace(connectionSettings.TokenPin))
                throw new ProblemDetailsException(
                    statusCode: StatusCodes.Status404NotFound,
                    message: "TOKEN_PIN_NOT_FOUND",
                    detail: "Token pin not found."
                    );

            IssuerDTO? issuerData = await _settingsStepService.GetIssuerData();

            if (issuerData is null)
                throw new ProblemDetailsException(
                    statusCode: StatusCodes.Status404NotFound,
                    message: "ISSUER_NOT_FOUND",
                    detail: "Issuer data not found."
                    );

            IssuerModel? issuer = issuerData.FromDTO();

            if (issuer is null)
                throw new ProblemDetailsException(
                    statusCode: StatusCodes.Status500InternalServerError,
                    message: "ISSUER_MAPPING_FAILED",
                    detail: "Issuer mapping failed"
                    );

            try
            {
                var url = _customConfig.Consumer_APIBaseUrl;
                string[] parts = url.Split('.');
                bool isProduction = false;

                if (parts.Length > 1)
                    isProduction = parts[1] != "preprod";

                SigningPropertiesModel signingProperties = new SigningPropertiesModel
                {
                    Documents = request.Invoices,
                    Issuer = issuer,
                    ItemCode = _customConfig.ItemCode,
                    InvoiceType = request.InvoiceType,
                    TokenPin = connectionSettings.TokenPin,
                    IsProduction = isProduction,
                    ForNotes = request.ForNotes
                };

                if (isProduction)
                    documents = await _documentSignerService.SignMultipleDocuments(signingProperties);
                else
                    documents = await _documentSignerService.SignMultipleDocumentsMock(signingProperties);
            }
            catch (Exception ex)
            {
                if (ex is ProblemDetailsException)
                    throw;
                else
                    throw new ProblemDetailsException(
                        statusCode: StatusCodes.Status500InternalServerError,
                        message: "SIGNING_ERR",
                        detail: $"Document signing failed. {ex.Message}"
                        );
            }

            string combinedJson = "{ \"documents\": [" + string.Join(",", documents) + "] }";

            genericRequest.Request = new RestRequest("/api/v1/documentsubmissions", Method.Post)
                            .AddHeader("Content-Type", "application/json").AddStringBody(combinedJson, ContentType.Json);
            genericRequest.ClientType = ClientType.Consumer;

            return genericRequest;
        }

        public GenericRequest GetRecentDocuments()
        {
            GenericRequest genericRequest = new();

            DateTime utcNow = DateTime.UtcNow;
            // Create a new DateTime without milliseconds
            DateTime trimmedUtcNow = new DateTime(
                utcNow.Year,
                utcNow.Month,
                utcNow.Day,
                utcNow.Hour,
                utcNow.Minute,
                utcNow.Second,
                DateTimeKind.Utc
            );

            genericRequest.Request = new RestRequest("/api/v1/documents/recent", Method.Get)
                .AddHeader("Accept", "application/json")
                .AddQueryParameter("pageNo", 1)
                .AddQueryParameter("pageSize", 100)
                .AddQueryParameter("submissionDateFrom", trimmedUtcNow.AddMonths(-1).ToString())
                .AddQueryParameter("submissionDateTo", trimmedUtcNow.ToString())
                .AddQueryParameter("documentType", "i");
            genericRequest.ClientType = ClientType.Consumer;

            return genericRequest;
        }

        public GenericRequest GetProviderInvoices(ProviderInvoicesSearchDTO searchModel)
        {
            GenericRequest genericRequest = new();

            genericRequest.Request = new RestRequest("/api/Invoices/GetInvoices", Method.Post);

            genericRequest.Request.AddJsonBody(searchModel);

            genericRequest.ClientType = ClientType.Provider;

            return genericRequest;
        }

        public GenericRequest GetProviderNotes(ProviderInvoicesSearchDTO searchModel)
        {
            GenericRequest genericRequest = new();

            genericRequest.Request = new RestRequest("/api/Invoices/GetNotes", Method.Post);

            genericRequest.Request.AddJsonBody(searchModel);

            genericRequest.ClientType = ClientType.Provider;

            return genericRequest;
        }

        public GenericRequest GetSubmission(string uuid, int pageNo, int pageSize)
        {
            GenericRequest genericRequest = new();
            genericRequest.Request = new RestRequest($"/api/v1/documentsubmissions/{uuid}", Method.Get)
                .AddParameter("pageNo", pageNo, ParameterType.QueryString)
                .AddParameter("pageSize", pageSize, ParameterType.QueryString);
            genericRequest.ClientType = ClientType.Consumer;

            return genericRequest;
        }

        public GenericRequest SearchDocuments(DateTime submissionDateFrom, DateTime submissionDateTo, string status, string receiverType, string direction)
        {
            submissionDateTo = submissionDateTo.AddHours(23).AddMinutes(59).AddSeconds(59);

            GenericRequest genericRequest = new();
            genericRequest.Request = new RestRequest("/api/v1/documents/search", Method.Get)
                .AddQueryParameter("documentType", "i")
                .AddQueryParameter("submissionDateFrom", submissionDateFrom)
                .AddQueryParameter("submissionDateTo", submissionDateTo)
                .AddQueryParameter("status", status)
                .AddQueryParameter("receiverType", receiverType)
                .AddQueryParameter("direction", direction);
            genericRequest.ClientType = ClientType.Consumer;

            return genericRequest;
        }

        public GenericRequest GetDocument(string uuid)
        {
            GenericRequest genericRequest = new();
            genericRequest.Request = new RestRequest("/api/v1/documents/{documentUUID}/raw", Method.Get)
                .AddUrlSegment("documentUUID", uuid);
            genericRequest.ClientType = ClientType.Consumer;

            return genericRequest;
        }
    }
}
