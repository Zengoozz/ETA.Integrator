using ETA.Integrator.Server.Interface.Services;
using ETA.Integrator.Server.Models.Consumer.ETA;
using ETA.Integrator.Server.Models.Provider;
using ETA.Integrator.Server.Dtos;
using ETA.Integrator.Server.Models.Core;
using ETA.Integrator.Server.Extensions;
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

            IssuerModel? issuer = issuerData.ToIssuerModel();

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

        public InvoiceModel PrepareInvoiceDetails(ProviderInvoiceViewModel invoiceViewModel, IssuerModel issuer, string tokenPin)
        {
            InvoiceModel document = new InvoiceModel();

            #region RECEIVER_PREP

            // BUILDING NUMBER | STREET | REGION/CITY | GOVERNATE | COUNTRY
            List<string> address = invoiceViewModel.ReceiverAddress.Split('|').ToList();

            ReceiverModel receiver = new ReceiverModel();

            receiver.Type = "B";
            receiver.Id = invoiceViewModel.RegistrationNumber == "NOT_FOUND" ? "313717919" : invoiceViewModel.RegistrationNumber;
            receiver.Name = invoiceViewModel.ReceiverName;
            receiver.Address = new ReceiverAddressModel
            {
                BuildingNumber = address[0],
                Street = address[1],
                RegionCity = address[2],
                Governate = address[3],
                Country = address[4].Trim()
            };

            #endregion

            #region INVOICE_LINE_PREP

            //List<InvoiceLineModel> invoiceLineList = new List<InvoiceLineModel>();
            foreach (var line in invoiceViewModel.InvoiceItems)
            {
                line.TaxableItems = [];
                line.InternalCode = "";
                if (line.TaxableItems.Count == 0)
                {
                    line.Discount = new DiscountModel
                    {
                        Rate = 0,
                        Amount = 0
                    };
                    line.ItemsDiscount = 0;
                    line.Total = line.NetTotal;
                    line.SalesTotal = line.NetTotal;
                    line.UnitValue.AmountEGP = line.NetTotal;
                }
            }
            #endregion

            #region TAX_TOTAL_PREP

            List<TaxTotalModel> taxTotalList = new List<TaxTotalModel>();

            //TaxTotalModel taxTotalModel = new TaxTotalModel();

            //taxTotalList.Add(taxTotalModel);

            #endregion

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
            ).AddMinutes(-1);

            _logger.LogInformation("INFO: Invoice DateTime: {DateTime}", trimmedUtcNow);
        
            document.Issuer = issuer;
            document.Receiver = receiver;
            document.TaxTotals = taxTotalList;
            document.Signatures = new List<SignatureModel>();
            document.DocumentType = "i";
            document.DocumentTypeVersion = "0.9";
            document.DateTimeIssued = trimmedUtcNow;
            document.TaxpayerActivityCode = "8610"; // HOSPITAL ACTIVITIES CODE
            document.InternalID = invoiceViewModel.InvoiceId.ToString();
            document.InvoiceLines = invoiceViewModel.InvoiceItems;
            document.NetAmount = invoiceViewModel.NetPrice;
            document.TotalSalesAmount = invoiceViewModel.InvoiceItems.Sum(i => i.SalesTotal); // SUM INVOICE LINES SALES
            document.TotalAmount = invoiceViewModel.NetPrice + taxTotalList.Sum(x => x.Amount); // NET + TOTAL TAX
            document.TotalDiscountAmount = invoiceViewModel.InvoiceItems.Sum(i => i.Discount.Amount); // SUM INVOICE LINES DISCOUNTS
            document.TotalItemsDiscountAmount = document.TotalDiscountAmount + document.ExtraDiscountAmount; // ? SAME AS TOTAL DISCOUNT AMOUNT ????
            document.ExtraDiscountAmount = 0; // DISCOUNT OVERALL DOCUMENT
            //document.purchaseOrderReference = ; // OPTIONAL
            //document.purchaseOrderDescription = ; // OPTIONAL
            //document.salesOrderReference = ; // OPTIONAL
            //document.salesOrderDescription = ; // OPTIONAL
            //document.proformaInvoiceNumber = ; // OPTIONAL
            //document.payment = ; // OPTIONAL
            //document.delivery = ; // OPTIONAL
            //document.ServiceDeliveryDate = ; //OPTIONAL

            #region SIGNATURES_PREP
            
            

            _signatureService.SignDocument(document, tokenPin);

            #endregion

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
