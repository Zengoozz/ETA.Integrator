using ETA.Integrator.Server.Helpers;
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
        public static InvoiceModel FromViewModel(this ProviderInvoiceViewModel viewModel, IssuerModel issuer)
        {
            if (viewModel == null)
                return new InvoiceModel();

            return new InvoiceModel
            {
                Issuer = issuer,
                Receiver = new ReceiverModel
                {
                    Type = "B",
                    Id = viewModel.RegistrationNumber == "NOT_FOUND" ? "313717919" : viewModel.RegistrationNumber,
                    Name = viewModel.ReceiverName,
                    Address = viewModel.ReceiverAddress
                },
                TaxTotals = new List<TaxTotalModel>(),
                Signatures = new List<SignatureModel>(),
                DocumentType = "i",
                DocumentTypeVersion = "0.9",
                DateTimeIssued = GenericHelpers.GetCurrentUTCTime(-1),
                TaxpayerActivityCode = "8610",
                InternalID = viewModel.InvoiceId.ToString(),
                InvoiceLines = viewModel.InvoiceItems.Select(item => new InvoiceLineModel
                {
                    // TaxableItems equals empty list
                    TaxableItems = new List<TaxableItemModel>(),
                    InternalCode = "",
                    ItemsDiscount = 0,
                    Discount = new DiscountModel
                    {
                        Rate = 0,
                        Amount = 0
                    },
                    Total = item.NetTotal,
                    SalesTotal = item.NetTotal,
                    UnitValue = new ValueModel
                    {
                        AmountEGP = item.NetTotal
                    },
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
