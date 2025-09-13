﻿using ETA.Integrator.Server.Dtos;
using ETA.Integrator.Server.Helpers.Enums;
using ETA.Integrator.Server.Interface.Services;
using ETA.Integrator.Server.Interface.Services.Common;
using ETA.Integrator.Server.Interface.Services.Consumer;
using ETA.Integrator.Server.Models;
using ETA.Integrator.Server.Models.Consumer.ETA;
using ETA.Integrator.Server.Models.Core;
using ETA.Integrator.Server.Models.Provider;
using ETA.Integrator.Server.Models.Provider.Requests;
using Microsoft.Extensions.Options;
using RestSharp;

namespace ETA.Integrator.Server.Services.Common
{
    public class RequestFactoryService : IRequestFactoryService
    {
        private readonly CustomConfigurations _customConfig;
        private readonly ISettingsStepService _settingsStepService;
        private readonly ISignatureConsumerService _signatureConsumerService;
        public RequestFactoryService(
            IOptions<CustomConfigurations> customConfig,
            ISettingsStepService settingsStepService,
            ISignatureConsumerService signatureConsumerService
            )
        {
            _customConfig = customConfig.Value;
            _settingsStepService = settingsStepService ?? throw new ArgumentNullException(nameof(_settingsStepService));
            _signatureConsumerService = signatureConsumerService;
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

        #region SUBMIT INVOICE
        public async Task<GenericRequest> SubmitDocuments(InvoiceRequest request)
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

            foreach (var invoice in request.Invoices)
            {
                var doc = PrepareInvoiceDetails(invoice, issuer, connectionSettings.TokenPin, request.InvoiceType);

                documents.Add(doc);
            }

            string combinedJson = "{ \"documents\": [" + string.Join(",", documents) + "] }";

            genericRequest.Request = new RestRequest("/api/v1/documentsubmissions", Method.Post)
                            .AddHeader("Content-Type", "application/json").AddStringBody(combinedJson , ContentType.Json);
            genericRequest.ClientType = ClientType.Consumer;

            return genericRequest;
        }

        private string PrepareInvoiceDetails(ProviderInvoiceViewModel invoiceViewModel, IssuerModel issuer, string tokenPin, string invoiceType)
        {

            //TODO: Rework this
            var url = _customConfig.Consumer_APIBaseUrl;
            string[] parts = url.Split('.');
            bool isProduction = false;
            if (parts.Length > 1)
                isProduction = parts[1] != "preprod";

            var document = invoiceViewModel.FromViewModel(issuer, invoiceType, isProduction);
            
            var signedDocument = _signatureConsumerService.SignDocument(document, tokenPin);
            if (signedDocument == null) throw new Exception("Invoice cannot be empty");
            return signedDocument;
        }

        #endregion

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
                .AddHeader("Content-Type", "application/json")
                .AddQueryParameter("pageNo", 1)
                .AddQueryParameter("pageSize", 100)
                .AddQueryParameter("submissionDateFrom", trimmedUtcNow.AddMonths(-1).ToString())
                .AddQueryParameter("submissionDateTo", trimmedUtcNow.ToString())
                .AddQueryParameter("documentType", "i");
            genericRequest.ClientType = ClientType.Consumer;

            return genericRequest;
        }

        public GenericRequest GetProviderInvoices(DateTime? fromDate, DateTime? toDate, string invoiceType)
        {
            GenericRequest genericRequest = new();

            genericRequest.Request = new RestRequest("/api/Invoices/GetInvoices", Method.Get);

            if (fromDate != null && toDate != null && !string.IsNullOrWhiteSpace(invoiceType))
            {
                genericRequest.Request.AddParameter("fromDate", fromDate, ParameterType.QueryString)
                    .AddParameter("toDate", toDate, ParameterType.QueryString)
                    .AddParameter("invoiceType", invoiceType, ParameterType.QueryString);
            }
            else
                throw new ProblemDetailsException(
                    StatusCodes.Status400BadRequest,
                    "INVALID_PARAMS",
                    "Please provide fromDate, toDate and invoiceType parameters."
                    );

            genericRequest.ClientType = ClientType.Provider;

            return genericRequest;
        }

    }
}
