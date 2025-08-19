using ETA.Integrator.Server.Dtos;
using ETA.Integrator.Server.Interface.Services;
using ETA.Integrator.Server.Interface.Services.Common;
using ETA.Integrator.Server.Interface.Services.Consumer;
using ETA.Integrator.Server.Models;
using ETA.Integrator.Server.Models.Consumer.ETA;
using ETA.Integrator.Server.Models.Core;
using ETA.Integrator.Server.Models.Provider;
using ETA.Integrator.Server.Models.Provider.Requests;
using RestSharp;
using System.Net;

namespace ETA.Integrator.Server.Services.Common
{
    public class RequestFactoryService : IRequestFactoryService
    {
        private readonly ILogger<RequestFactoryService> _logger;
        private readonly ISettingsStepService _settingsStepService;
        private readonly ISignatureConsumerService _signatureConsumerService;
        public RequestFactoryService(
            ILogger<RequestFactoryService> logger,
            ISettingsStepService settingsStepService,
            ISignatureConsumerService signatureConsumerService
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settingsStepService = settingsStepService ?? throw new ArgumentNullException(nameof(_settingsStepService));
            _signatureConsumerService = signatureConsumerService;
        }

        public RestRequest ConnectToProvider(ProviderLoginRequestModel model)
        {
            var request = new RestRequest("/api/Auth/LogIn", Method.Post).AddJsonBody(model);
            return request;
        }

        public async Task<RestRequest> ConnectToConsumer(ConnectionDTO? model)
        {
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


            var request = new RestRequest("/connect/token", Method.Post)
                .AddParameter("grant_type", "client_credentials")
                .AddParameter("client_id", connectionConfig.ClientId)
                .AddParameter("client_secret", connectionConfig.ClientSecret)
                .AddParameter("scope", "InvoicingAPI");

            return request;
        }

        #region SUBMIT INVOICE
        public async Task<RestRequest> SubmitDocuments(List<ProviderInvoiceViewModel> invoicesList)
        {
            List<InvoiceModel> documents = new List<InvoiceModel>();

            var connectionSettings = await _settingsStepService.GetConnectionData();

            if (connectionSettings is null || string.IsNullOrWhiteSpace(connectionSettings.TokenPin))
                throw new ProblemDetailsException(
                    statusCode: StatusCodes.Status404NotFound,
                    message: "TOKEN_PIN_NOT_FOUND",
                    detail: "Token pin not found."
                    );

            #region ISSUER_PREP

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
            #endregion

            #region INVOICE_PREP
            foreach (var invoice in invoicesList)
            {
                var doc = PrepareInvoiceDetails(invoice, issuer, connectionSettings.TokenPin);

                documents.Add(doc);
            }
            #endregion

            #region REQUEST_PREPARE
            var submitRequestBody = new
            {
                documents
            };

            var submitRequest = new RestRequest("/api/v1/documentsubmissions", Method.Post)
                            .AddHeader("Content-Type", "application/json").AddJsonBody(submitRequestBody);
            #endregion

            return submitRequest;
        }

        private InvoiceModel PrepareInvoiceDetails(ProviderInvoiceViewModel invoiceViewModel, IssuerModel issuer, string tokenPin)
        {

            //TODO: Validate the invoices data integrity

            InvoiceModel document = invoiceViewModel.FromViewModel(issuer);

            //_signatureConsumerService.SignDocument(document, tokenPin);

            return document;
        }

        #endregion

        public RestRequest GetRecentDocuments()
        {
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

            var request = new RestRequest("/api/v1/documents/recent", Method.Get)
                .AddHeader("Content-Type", "application/json")
                .AddQueryParameter("pageNo", 1)
                .AddQueryParameter("pageSize", 100)
                .AddQueryParameter("submissionDateFrom", trimmedUtcNow.AddMonths(-1).ToString())
                .AddQueryParameter("submissionDateTo", trimmedUtcNow.ToString())
                .AddQueryParameter("documentType", "i");

            return request;
        }

        public RestRequest GetProviderInvoices(DateTime? fromDate, DateTime? toDate, string invoiceType)
        {
            var request = new RestRequest("/api/Invoices/GetInvoices", Method.Get);

            if (fromDate != null && toDate != null && !string.IsNullOrWhiteSpace(invoiceType))
            {
                request.AddParameter("fromDate", fromDate, ParameterType.QueryString)
                    .AddParameter("toDate", toDate, ParameterType.QueryString)
                    .AddParameter("invoiceType", invoiceType, ParameterType.QueryString);
            }
            else
                throw new ProblemDetailsException(
                    StatusCodes.Status400BadRequest,
                    "INVALID_PARAMS",
                    "Please provide fromDate, toDate and invoiceType parameters."
                    );

            return request;
        }

    }
}
