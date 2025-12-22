using ETA.Integrator.Server.Models.Consumer.ETA;

namespace ETA.Integrator.Server.Dtos.SerializationDTOs
{
    public class CreditNoteToSerializeDTO : InvoiceToSerializeDTO
    {
        public CreditNoteToSerializeDTO() { }
        public CreditNoteToSerializeDTO(InvoiceToSerializeDTO baseModel)
        {
            this.Issuer = baseModel.Issuer;
            this.Receiver = baseModel.Receiver;
            this.DocumentType = baseModel.DocumentType;
            this.DocumentTypeVersion = baseModel.DocumentTypeVersion;
            this.DateTimeIssued = baseModel.DateTimeIssued;
            this.TaxpayerActivityCode = baseModel.TaxpayerActivityCode;
            this.InternalID = baseModel.InternalID;
            this.PurchaseOrderReference = baseModel.PurchaseOrderReference;
            this.PurchaseOrderDescription = baseModel.PurchaseOrderDescription;
            this.SalesOrderReference = baseModel.SalesOrderReference;
            this.SalesOrderDescription = baseModel.SalesOrderDescription;
            this.ProformaInvoiceNumber = baseModel.ProformaInvoiceNumber;
            this.Payment = baseModel.Payment;
            this.Delivery = baseModel.Delivery;
            this.InvoiceLines = baseModel.InvoiceLines;
            this.TotalSalesAmount = baseModel.TotalSalesAmount;
            this.TotalDiscountAmount = baseModel.TotalDiscountAmount;
            this.NetAmount = baseModel.NetAmount;
            this.TaxTotals = baseModel.TaxTotals;
            this.ExtraDiscountAmount = baseModel.ExtraDiscountAmount;
            this.TotalItemsDiscountAmount = baseModel.TotalItemsDiscountAmount;
            this.TotalAmount = baseModel.TotalAmount;
        }
        public List<string> References { get; set; } = new();
    }
}
