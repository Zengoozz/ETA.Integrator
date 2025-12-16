using ETA.Integrator.Server.Helpers;
using ETA.Integrator.Server.Interface.Services;
using ETA.Integrator.Server.Models.Core;

namespace ETA.Integrator.Server.Models.Consumer.ETA
{
    public class CreditNoteModel : InvoiceModel
    {
        public CreditNoteModel() { }
        public CreditNoteModel(InvoiceModel baseModel)
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
            this.Signatures = baseModel.Signatures;
            this.ServiceDeliveryDate = baseModel.ServiceDeliveryDate;
        }
        public List<string> References { get; set; } = new();
    }

    public class CreditNoteModelMapper : IDocumentMapper
    {
        public InvoiceModel BaseMap(DocumentMappIngPropertiesModel mappingProperties)
        {
            if (String.IsNullOrEmpty(mappingProperties.Document.ReferenceId))
                throw new ProblemDetailsException(
                    statusCode: StatusCodes.Status400BadRequest,
                    message: "INVALID",
                    detail: "Credit notes should have "
                    );


            InvoiceModel baseModel = GenericHelpers.MapBaseDocument(mappingProperties);

            return new CreditNoteModel(baseModel)
            {
                DocumentType = "c",
                References = mappingProperties.References
            };
        }
    }
}
