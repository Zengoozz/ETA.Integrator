﻿using ETA.Integrator.Server.Interface.Services;
using ETA.Integrator.Server.Models.Consumer.ETA;
using ETA.Integrator.Server.Models.Provider;
using ETA.Integrator.Server.Dtos;
using ETA.Integrator.Server.Models.Core;
using RestSharp;
using ETA.Integrator.Server.Interface.Services.Consumer;

namespace ETA.Integrator.Server.Services.Consumer
{
    public class RequestFactoryConsumerService : IRequestFactoryConsumerService
    {
        private readonly ILogger<RequestFactoryConsumerService> _logger;
        private readonly ISettingsStepService _settingsStepService;
        private readonly ISignatureConsumerService _signatureConsumerService;
        public RequestFactoryConsumerService(
            ILogger<RequestFactoryConsumerService> logger,
            ISettingsStepService settingsStepService,
            ISignatureConsumerService signatureConsumerService
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settingsStepService = settingsStepService ?? throw new ArgumentNullException(nameof(_settingsStepService));
            _signatureConsumerService = signatureConsumerService;
        }

        #region SUBMIT INVOICE
        public async Task<RestRequest> SubmitInvoices(List<ProviderInvoiceViewModel> invoicesList)
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

            _signatureConsumerService.SignDocument(document, tokenPin);

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
    }
}
