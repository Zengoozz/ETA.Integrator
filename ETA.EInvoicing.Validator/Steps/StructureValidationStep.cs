using ETA.EInvoicing.Validator.Enums;
using ETA.EInvoicing.Validator.Interfaces;
using ETA.EInvoicing.Validator.Models;
using System.Text.Json;

namespace ETA.EInvoicing.Validator.Steps;

public class StructureValidationStep : IValidationStep
{
    public async Task<StepErrorDto> ValidateAsync(JsonDocument invoiceJson, int stepNumber)
    {
        var errors = new List<ValidationErrorDto>();
        var root = invoiceJson.RootElement;

        void EnsureProperty(JsonElement parent, string name, JsonValueKind expectedKind, bool required = true)
        {
            if (!parent.TryGetProperty(name, out var prop))
            {
                if (required)
                    errors.Add(new ValidationErrorDto { Key = name, Message = $"Missing required property '{name}'." });
            }
            else if (prop.ValueKind != expectedKind)
            {
                errors.Add(new ValidationErrorDto { Key = name, Message = $"Property '{name}' must be of type {expectedKind}." });
            }
        }

        // Root required fields
        EnsureProperty(root, "issuer", JsonValueKind.Object);
        EnsureProperty(root, "receiver", JsonValueKind.Object);
        EnsureProperty(root, "documentType", JsonValueKind.String);
        EnsureProperty(root, "documentTypeVersion", JsonValueKind.String);
        EnsureProperty(root, "dateTimeIssued", JsonValueKind.String);
        EnsureProperty(root, "taxpayerActivityCode", JsonValueKind.String);
        EnsureProperty(root, "internalID", JsonValueKind.String);
        EnsureProperty(root, "payment", JsonValueKind.Object);
        EnsureProperty(root, "delivery", JsonValueKind.Object);
        EnsureProperty(root, "invoiceLines", JsonValueKind.Array);
        EnsureProperty(root, "totalDiscountAmount", JsonValueKind.Number);
        EnsureProperty(root, "totalSalesAmount", JsonValueKind.Number);
        EnsureProperty(root, "netAmount", JsonValueKind.Number);
        EnsureProperty(root, "taxTotals", JsonValueKind.Array);
        EnsureProperty(root, "totalAmount", JsonValueKind.Number);
        EnsureProperty(root, "signatures", JsonValueKind.Array);
        EnsureProperty(root, "serviceDeliveryDate", JsonValueKind.String); // required date

        // Optional fields
        EnsureProperty(root, "purchaseOrderReference", JsonValueKind.String, required: false);
        EnsureProperty(root, "purchaseOrderDescription", JsonValueKind.String, required: false);
        EnsureProperty(root, "salesOrderReference", JsonValueKind.String, required: false);
        EnsureProperty(root, "salesOrderDescription", JsonValueKind.String, required: false);
        EnsureProperty(root, "proformaInvoiceNumber", JsonValueKind.String, required: false);
        EnsureProperty(root, "extraDiscountAmount", JsonValueKind.Number, required: false);
        EnsureProperty(root, "totalItemsDiscountAmount", JsonValueKind.Number, required: false);

        // Issuer nested
        if (root.TryGetProperty("issuer", out var issuer) && issuer.ValueKind == JsonValueKind.Object)
        {
            EnsureProperty(issuer, "type", JsonValueKind.String);
            EnsureProperty(issuer, "id", JsonValueKind.String);
            EnsureProperty(issuer, "name", JsonValueKind.String);
            EnsureProperty(issuer, "address", JsonValueKind.Object);
        }

        // Receiver nested
        if (root.TryGetProperty("receiver", out var receiver) && receiver.ValueKind == JsonValueKind.Object)
        {
            EnsureProperty(receiver, "type", JsonValueKind.String);
            EnsureProperty(receiver, "id", JsonValueKind.String, required: false); // may vary by type
            EnsureProperty(receiver, "name", JsonValueKind.String, required: false);
            EnsureProperty(receiver, "address", JsonValueKind.Object);
        }

        // InvoiceLines nested
        if (root.TryGetProperty("invoiceLines", out var lines) && lines.ValueKind == JsonValueKind.Array)
        {
            if (lines.GetArrayLength() == 0)
                errors.Add(new ValidationErrorDto { Key = "invoiceLines", Message = "invoiceLines must contain at least one item." });

            foreach (var line in lines.EnumerateArray())
            {
                EnsureProperty(line, "description", JsonValueKind.String);
                EnsureProperty(line, "itemType", JsonValueKind.String);
                EnsureProperty(line, "itemCode", JsonValueKind.String);
                EnsureProperty(line, "unitType", JsonValueKind.String);
                EnsureProperty(line, "quantity", JsonValueKind.Number);
                EnsureProperty(line, "salesTotal", JsonValueKind.Number);
                EnsureProperty(line, "total", JsonValueKind.Number);

                if (line.TryGetProperty("unitValue", out var unitValue) && unitValue.ValueKind == JsonValueKind.Object)
                {
                    EnsureProperty(unitValue, "amountEGP", JsonValueKind.Number);
                    EnsureProperty(unitValue, "currencySold", JsonValueKind.String, required: false);
                    EnsureProperty(unitValue, "amountSold", JsonValueKind.Number, required: false);
                    EnsureProperty(unitValue, "exchangeRate", JsonValueKind.Number, required: false);
                }
            }
        }

        // TaxTotals nested
        if (root.TryGetProperty("taxTotals", out var taxTotals) && taxTotals.ValueKind == JsonValueKind.Array)
        {
            foreach (var tax in taxTotals.EnumerateArray())
            {
                EnsureProperty(tax, "taxType", JsonValueKind.String);
                EnsureProperty(tax, "amount", JsonValueKind.Number);
                EnsureProperty(tax, "subType", JsonValueKind.String, required: false);
            }
        }

        // Signatures nested
        if (root.TryGetProperty("signatures", out var signatures) && signatures.ValueKind == JsonValueKind.Array)
        {
            foreach (var sig in signatures.EnumerateArray())
            {
                EnsureProperty(sig, "signatureType", JsonValueKind.String);
                EnsureProperty(sig, "value", JsonValueKind.String);
            }
        }

        return await Task.FromResult(new StepErrorDto
        {
            StepNumber = stepNumber,
            StepName = "Step_1",
            ValidationType = InvoiceValidationType.Structure.ToString(),
            Errors = errors
        });
    }
}
