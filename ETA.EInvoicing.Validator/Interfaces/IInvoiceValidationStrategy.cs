using ETA.EInvoicing.Validator.Models;

namespace ETA.EInvoicing.Validator.Interfaces;

internal interface IInvoiceValidationStrategy
{
    Task<List<StepErrorDto>> ValidateAsync(string invoiceJson);
}
