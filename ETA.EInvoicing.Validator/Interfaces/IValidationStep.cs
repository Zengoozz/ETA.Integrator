using ETA.EInvoicing.Validator.Models;
using System.Text.Json;

namespace ETA.EInvoicing.Validator.Interfaces;

internal interface IValidationStep
{
    Task<StepErrorDto> ValidateAsync(JsonDocument invoiceJson, int stepNumber);
}
