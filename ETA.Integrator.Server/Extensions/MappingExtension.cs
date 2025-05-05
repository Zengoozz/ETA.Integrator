using ETA.Integrator.Server.Dtos;
using ETA.Integrator.Server.Models.Consumer.ETA;

namespace ETA.Integrator.Server.Extensions
{
    public static class MappingExtension
    {
        public static IssuerModel? ToIssuerModel(this IssuerDTO dto)
        {
            if (dto == null)
                return null;

            return new IssuerModel
            {
                Type = dto.IssuerType,
                Id = dto.RegistrationNumber,
                Name = dto.IssuerName,
                Address = dto.Address
            };
        }

        public static InvoiceToSerializeDTO ToDTO(this InvoiceModel dto)
        {
            if (dto == null)
                return new InvoiceToSerializeDTO();

            return new InvoiceToSerializeDTO
            {
                Issuer = dto.Issuer,
                Receiver = dto.Receiver,
                DocumentType = dto.DocumentType,
                DocumentTypeVersion = dto.DocumentTypeVersion,
                DateTimeIssued = dto.DateTimeIssued,
                TaxpayerActivityCode = dto.TaxpayerActivityCode,
                InternalID = dto.InternalID,
                PurchaseOrderReference = dto.PurchaseOrderReference,
                PurchaseOrderDescription = dto.PurchaseOrderDescription,
                SalesOrderReference = dto.SalesOrderReference,
                SalesOrderDescription = dto.SalesOrderDescription,
                ProformaInvoiceNumber = dto.ProformaInvoiceNumber,
                Payment = dto.Payment,
                Delivery = dto.Delivery,
                InvoiceLines = dto.InvoiceLines,
                TotalDiscountAmount = dto.TotalDiscountAmount,
                TotalSalesAmount = dto.TotalSalesAmount,
                NetAmount = dto.NetAmount,
                TaxTotals = dto.TaxTotals,
                TotalAmount = dto.TotalAmount,
                ExtraDiscountAmount = dto.ExtraDiscountAmount,
                TotalItemsDiscountAmount = dto.TotalItemsDiscountAmount,
            };
        }
    }
}
