using ETA.Integrator.Server.Helpers;
using ETA.Integrator.Server.Models.Core;
using ETA.Integrator.Server.Models.Provider;

namespace ETA.Integrator.Server.Models.Consumer.ETA
{
    public class InvoiceModel
    {
        public IssuerModel Issuer { get; set; } = new IssuerModel();
        public ReceiverModel Receiver { get; set; } = new ReceiverModel();
        public string DocumentType { get; set; } = string.Empty; // Must be "i"
        public string DocumentTypeVersion { get; set; } = string.Empty; // Must be "1.0"
        public DateTime DateTimeIssued { get; set; }
        public string TaxpayerActivityCode { get; set; } = string.Empty;
        public string InternalID { get; set; } = string.Empty;
        public string PurchaseOrderReference { get; set; } = string.Empty;
        public string PurchaseOrderDescription { get; set; } = string.Empty;
        public string SalesOrderReference { get; set; } = string.Empty;
        public string SalesOrderDescription { get; set; } = string.Empty;
        public string ProformaInvoiceNumber { get; set; } = string.Empty;
        public PaymentModel Payment { get; set; } = new PaymentModel();
        public DeliveryModel Delivery { get; set; } = new DeliveryModel();
        public List<InvoiceLineModel> InvoiceLines { get; set; } = new List<InvoiceLineModel>();
        public decimal TotalSalesAmount { get; set; }
        public decimal TotalDiscountAmount { get; set; }
        public decimal NetAmount { get; set; }
        public List<TaxTotalModel> TaxTotals { get; set; } = new List<TaxTotalModel>();
        public decimal ExtraDiscountAmount { get; set; }
        public decimal TotalItemsDiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public List<SignatureModel> Signatures { get; set; } = new List<SignatureModel>();
        public DateTime? ServiceDeliveryDate { get; set; }
    }

    public static class InvoiceModelMapper
    {
        public static InvoiceModel FromViewModel(this ProviderInvoiceViewModel viewModel, IssuerModel issuer, bool isProduction = false)
        {
            if (viewModel is null)
                throw new ProblemDetailsException(
                    statusCode: StatusCodes.Status400BadRequest,
                    message: "PROVIDER_INVOICE_NULL",
                    detail: "Mapping provider invoice to the consumer invoice failed!"
                    );

            if (viewModel.RegistrationNumber == "NOT_FOUND" || string.IsNullOrEmpty(viewModel.RegistrationNumber))
                throw new ProblemDetailsException(
                   statusCode: StatusCodes.Status400BadRequest,
                   message: "NOT_FOUND",
                   detail: $"Invoice #{viewModel.InvoiceNumber}: Reciever (${viewModel.ReceiverName}) has no registeration number."
                   );

            var listOfNeededProps = new List<string> { "Country", "Governate", "RegionCity", "Street", "BuildingNumber" };

            var receiverAddressObjDict = viewModel.ReceiverAddress.GetType()
                 .GetProperties()
                 .ToDictionary(p => p.Name, p => p.GetValue(viewModel.ReceiverAddress));

            var isAddressCorrupt = receiverAddressObjDict.Any(d => listOfNeededProps.Contains(d.Key) && d.Value is null);

            if (isAddressCorrupt)
                throw new ProblemDetailsException(
                       statusCode: StatusCodes.Status400BadRequest,
                       message: "INVALID",
                       detail: $"Invoice #{viewModel.InvoiceNumber}: Reciever ({viewModel.ReceiverName}) has invalid address."
                       );

            return new InvoiceModel
            {
                Issuer = issuer,
                Receiver = new ReceiverModel
                {
                    Type = "B",
                    Id = viewModel.RegistrationNumber,
                    Name = viewModel.ReceiverName,
                    Address = viewModel.ReceiverAddress
                },
                TaxTotals = new List<TaxTotalModel>(),
                Signatures = new List<SignatureModel>(),
                DocumentType = "i",
                DocumentTypeVersion = isProduction ? "1.0" : "0.9",
                DateTimeIssued = GenericHelpers.GetCurrentUTCTime(-1),
                TaxpayerActivityCode = "8610",
                InternalID = viewModel.InvoiceId.ToString(),
                InvoiceLines = viewModel.InvoiceItems.Select(item => new InvoiceLineModel
                {
                    Description = item.Description,
                    ItemType = item.ItemType,
                    ItemCode = item.ItemCode,
                    UnitType = item.UnitType,
                    Quantity = item.Quantity,
                    UnitValue = item.UnitValue,
                    SalesTotal = item.NetTotal,
                    NetTotal = item.NetTotal,
                    Total = item.NetTotal,
                    ItemsDiscount = item.ItemsDiscount,
                    ValueDifference = item.ValueDifference,
                    TotalTaxableFees = item.TotalTaxableFees,
                    InternalCode = item.InternalCode,
                    Discount = item.Discount,
                }).ToList(),
                NetAmount = viewModel.NetPrice,
                TotalSalesAmount = viewModel.InvoiceItems.Sum(i => i.NetTotal),
                TotalAmount = viewModel.NetPrice + 0, // Based on the Sum of TaxTotals.Amount
                TotalDiscountAmount = 0, // Based on the Sum of InvoiceLines Discount.Amount
                ExtraDiscountAmount = 0,
                TotalItemsDiscountAmount = 0, // Based on the Sum of TotalDiscountAmount and ExtraDiscountAmount
                                              //document.purchaseOrderReference = ; // OPTIONAL
                                              //document.purchaseOrderDescription = ; // OPTIONAL
                                              //document.salesOrderReference = ; // OPTIONAL
                                              //document.salesOrderDescription = ; // OPTIONAL
                                              //document.proformaInvoiceNumber = ; // OPTIONAL
                                              //document.payment = ; // OPTIONAL
                                              //document.delivery = ; // OPTIONAL
                                              //document.ServiceDeliveryDate = ; //OPTIONAL
            };
        }
    }


}
