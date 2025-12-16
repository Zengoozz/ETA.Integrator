using ETA.Integrator.Server.Models;
using ETA.Integrator.Server.Models.Consumer.ETA;
using ETA.Integrator.Server.Models.Core;
using System.Text.Json;

namespace ETA.Integrator.Server.Helpers
{
    public static class GenericHelpers
    {
        public static DateTime GetCurrentUTCTime(int minutes)
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
            ).AddMinutes(minutes);
            return trimmedUtcNow;
        }

        public static (DateTime start, DateTime end) GetStartAndEndOfDay(DateTime date)
        {
            DateTime startOfDay = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Utc);
            DateTime endOfDay = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, DateTimeKind.Utc);
            return (startOfDay, endOfDay);
        }

        public static T JsonDeserialize<T>(string content, JsonSerializerOptions? opt = null) where T : new()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };

            if (opt is not null)
                options = opt;

            T? serializedResponse = new();

            try
            {
                serializedResponse = JsonSerializer.Deserialize<T>(content, options);
            }
            catch (Exception)
            {
                throw;
            }

            if (serializedResponse is null)
                throw new ProblemDetailsException(
                    statusCode: StatusCodes.Status500InternalServerError,
                    message: "SERIALIZATION_FAILED",
                    detail: "Could not serialize the response."
                    );

            return serializedResponse;
        }

        public static InvoiceModel MapBaseDocument(DocumentMappIngPropertiesModel mappingProperties)
        {
            if (mappingProperties.Document is null)
                throw new ProblemDetailsException(
                    statusCode: StatusCodes.Status400BadRequest,
                    message: "PROVIDER_INVOICE_NULL",
                    detail: "Mapping provider invoice to the consumer invoice failed!"
                    );

            if (mappingProperties.Document.RegistrationNumber == "NOT_FOUND" || string.IsNullOrEmpty(mappingProperties.Document.RegistrationNumber))
                throw new ProblemDetailsException(
                   statusCode: StatusCodes.Status400BadRequest,
                   message: "NOT_FOUND",
                   detail: $"Invoice #{mappingProperties.Document.InvoiceNumber}: Reciever ({mappingProperties.Document.ReceiverName}) has no registeration number."
                   );

            var listOfNeededProps = new List<string> { "Country", "Governate", "RegionCity", "Street", "BuildingNumber" };

            var receiverAddressObjDict = mappingProperties.Document.ReceiverAddress.GetType()
                 .GetProperties()
                 .ToDictionary(p => p.Name, p => p.GetValue(mappingProperties.Document.ReceiverAddress));

            var isAddressCorrupt = receiverAddressObjDict.Any(d => listOfNeededProps.Contains(d.Key) && d.Value is null);

            if (isAddressCorrupt)
                throw new ProblemDetailsException(
                       statusCode: StatusCodes.Status400BadRequest,
                       message: "INVALID",
                       detail: $"Invoice #{mappingProperties.Document.InvoiceNumber}: Reciever ({mappingProperties.Document.ReceiverName}) has invalid address."
                       );

            return new InvoiceModel
            {
                Issuer = mappingProperties.Issuer,
                Receiver = new ReceiverModel
                {
                    Type = mappingProperties.InvoiceType == "I" ? (mappingProperties.Document.ReceiverAddress.Country == "EG" ? "P" : "F") : "B",
                    Id = mappingProperties.Document.RegistrationNumber,
                    Name = mappingProperties.Document.ReceiverName,
                    Address = mappingProperties.Document.ReceiverAddress
                },
                TaxTotals = new List<TaxTotalModel>(),
                Signatures = new List<SignatureModel>(),
                DocumentType = "i",
                DocumentTypeVersion = mappingProperties.IsProduction ? "1.0" : "0.9",
                DateTimeIssued = GenericHelpers.GetCurrentUTCTime(-70).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                TaxpayerActivityCode = "8610",
                InternalID = mappingProperties.Document.InvoiceId,
                InvoiceLines = mappingProperties.Document.InvoiceItems.Select(item => new InvoiceLineModel
                {
                    Description = item.Description,
                    ItemType = item.ItemType,
                    ItemCode = mappingProperties.ItemCode,
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
                NetAmount = mappingProperties.Document.NetPrice,
                TotalSalesAmount = mappingProperties.Document.InvoiceItems.Sum(i => i.NetTotal),
                TotalAmount = mappingProperties.Document.NetPrice + 0, // Based on the Sum of TaxTotals.Amount
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