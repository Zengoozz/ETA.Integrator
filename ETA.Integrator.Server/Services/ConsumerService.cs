using ETA.Integrator.Server.Interface.Services;
using ETA.Integrator.Server.Models.Consumer.ETA;
using ETA.Integrator.Server.Models.Provider;
using ETA.Integrator.Server.Dtos;
using ETA.Integrator.Server.Models.Core;
using RestSharp;

namespace ETA.Integrator.Server.Services
{
    public class ConsumerService : IConsumerService
    {
        private readonly ILogger<ConsumerService> _logger;
        private readonly ISettingsStepService _settingsStepService;
        private readonly ISignatureService _signatureService;
        public ConsumerService(ILogger<ConsumerService> logger, ISettingsStepService settingsStepService, ISignatureService signatureService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settingsStepService = settingsStepService ?? throw new ArgumentNullException(nameof(_settingsStepService));
            _signatureService = signatureService;
        }

        #region SUBMIT INVOICE
        public async Task<RestRequest> SubmitInvoiceRequest(List<ProviderInvoiceViewModel> invoicesList)
        {
            List<InvoiceModel> documents = new List<InvoiceModel>();

            var connectionSettings = await _settingsStepService.GetConnectionData();

            if (String.IsNullOrWhiteSpace(connectionSettings.TokenPin))
                throw new ProblemDetailsException(
                    statusCode: StatusCodes.Status404NotFound,
                    message: "TOKEN_PIN_NOT_FOUND",
                    detail: "Token pin not found."
                    );

            #region ISSUER_PREP

            IssuerDTO? issuerData = await _settingsStepService.GetIssuerData();

            if (issuerData == null)
                throw new ProblemDetailsException(
                    statusCode: StatusCodes.Status404NotFound,
                    message: "ISSUER_NOT_FOUND",
                    detail: "Issuer data not found."
                    );

            IssuerModel? issuer = issuerData.FromDTO();

            if (issuer == null)
                throw new Exception("Issuer mapping failed");

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
            InvoiceModel document = invoiceViewModel.FromViewModel(issuer);

            _signatureService.SignDocument(document, tokenPin);

            return document;
        }

        #endregion

        public RestRequest GetRecentDocumentsRequest()
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
