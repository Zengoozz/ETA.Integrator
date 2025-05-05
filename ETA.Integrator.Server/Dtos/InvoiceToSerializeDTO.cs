using ETA.Integrator.Server.Models.Consumer.ETA;

namespace ETA.Integrator.Server.Dtos
{
    public class InvoiceToSerializeDTO
    {
        public IssuerModel Issuer { get; set; } = new IssuerModel();
        public ReceiverModel Receiver { get; set; } = new ReceiverModel();
        public string DocumentType { get; set; } = string.Empty;
        public string DocumentTypeVersion { get; set; } = string.Empty;
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
        public decimal TotalDiscountAmount { get; set; }
        public decimal TotalSalesAmount { get; set; }
        public decimal NetAmount { get; set; }
        public List<TaxTotalModel> TaxTotals { get; set; } = new List<TaxTotalModel>();
        public decimal TotalAmount { get; set; }
        public decimal ExtraDiscountAmount { get; set; }
        public decimal TotalItemsDiscountAmount { get; set; }
    }
}
