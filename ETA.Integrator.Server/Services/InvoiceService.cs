using ETA.Integrator.Server.Interface.Services;
using ETA.Integrator.Server.Models.Consumer.ETA;
using ETA.Integrator.Server.Models.Provider;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System.Security.Cryptography.Xml;

namespace ETA.Integrator.Server.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly ILogger<InvoiceService> _logger;
        public InvoiceService(ILogger<InvoiceService> logger, ISignatureService signatureService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public InvoiceModel PrepareInvoiceData(ProviderInvoiceViewModel invoiceViewModel, IssuerModel issuer)
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

            #region SIGNATURES_PREP

            List<SignatureModel> signatureList = new List<SignatureModel>();


            SignatureModel signature = new SignatureModel
            {
                SignatureType = "I",
                Value = "SignatureValue", // REQUIRED
            };

            signatureList.Add(signature);

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
            );

            document.Issuer = issuer;
            document.Receiver = receiver;
            document.TaxTotals = taxTotalList;
            document.Signatures = signatureList;
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

            return document;
        }

    }
}
