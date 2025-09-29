using ETA.EInvoicing.Validator.Enums;
using ETA.EInvoicing.Validator.Interfaces;
using ETA.EInvoicing.Validator.Models;
using System.Globalization;
using System.Text.Json;

namespace ETA.EInvoicing.Validator.Steps;


public class CoreFieldsValidationStep : IValidationStep
{
    public async Task<StepErrorDto> ValidateAsync(JsonDocument document, int stepNumber)
    {
        var stepErrors = new StepErrorDto
        {
            StepNumber = stepNumber,
            ValidationType = InvoiceValidationType.CoreFields.ToString(),
            StepName = "Step_2",
            Errors = new List<ValidationErrorDto>()
        };
        stepErrors.StepName = string.IsNullOrEmpty(stepErrors.StepName) ? "CoreFieldsValidator" : stepErrors.StepName;

        var root = document.RootElement;

        // helper lambdas
        bool HasProp(JsonElement el, string name) => el.TryGetProperty(name, out _);
        JsonValueKind PropKind(JsonElement el, string name)
        {
            if (el.TryGetProperty(name, out var p)) return p.ValueKind;
            return JsonValueKind.Undefined;
        }
        string ReadString(JsonElement el, string name) => el.TryGetProperty(name, out var p) && p.ValueKind == JsonValueKind.String ? p.GetString() ?? string.Empty : string.Empty;

        // 1) documentType (required) - must be 'i'|'c'|'d'
        if (!HasProp(root, "documentType"))
            stepErrors.Errors.Add(new ValidationErrorDto { Key = "documentType", Message = "Missing required 'documentType'." });
        else
        {
            var v = ReadString(root, "documentType");
            if (!(v == "i" || v == "c" || v == "d"))
                stepErrors.Errors.Add(new ValidationErrorDto { Key = "documentType", Message = "documentType must be one of: 'i' (invoice), 'c' (credit note), 'd' (debit note)." });
        }

        // 2) documentTypeVersion (required) - must be "1.0" for this version
        if (!HasProp(root, "documentTypeVersion"))
            stepErrors.Errors.Add(new ValidationErrorDto { Key = "documentTypeVersion", Message = "Missing required 'documentTypeVersion'." });
        else
        {
            var v = ReadString(root, "documentTypeVersion");
            if (!string.Equals(v, "1.0", StringComparison.OrdinalIgnoreCase))
                stepErrors.Errors.Add(new ValidationErrorDto { Key = "documentTypeVersion", Message = "documentTypeVersion must be '1.0' for this document version." });
        }

        // 3) dateTimeIssued (required) - ISO-8601 and not in future (UTC)
        if (!HasProp(root, "dateTimeIssued"))
        {
            stepErrors.Errors.Add(new ValidationErrorDto { Key = "dateTimeIssued", Message = "Missing required 'dateTimeIssued'." });
        }
        else
        {
            var s = ReadString(root, "dateTimeIssued");
            if (!DateTimeOffset.TryParse(s, null, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var dto))
            {
                stepErrors.Errors.Add(new ValidationErrorDto { Key = "dateTimeIssued", Message = "dateTimeIssued must be a valid ISO-8601 datetime string in UTC." });
            }
            else if (dto > DateTimeOffset.UtcNow.AddMinutes(5)) // allow small clock skew
            {
                stepErrors.Errors.Add(new ValidationErrorDto { Key = "dateTimeIssued", Message = "dateTimeIssued cannot be in the future." });
            }
        }

        // 4) taxpayerActivityCode (required string)
        if (!HasProp(root, "taxpayerActivityCode") || PropKind(root, "taxpayerActivityCode") != JsonValueKind.String)
            stepErrors.Errors.Add(new ValidationErrorDto { Key = "taxpayerActivityCode", Message = "Missing or invalid 'taxpayerActivityCode'." });

        // 5) internalId (required string)
        if (!HasProp(root, "internalId") && !HasProp(root, "internalID")) // some payloads use InternalID vs internalId
            stepErrors.Errors.Add(new ValidationErrorDto { Key = "internalId", Message = "Missing required 'internalId' (internal document id)." });
        else
        {
            // tolerate either casing
            var val = HasProp(root, "internalId") ? ReadString(root, "internalId") : ReadString(root, "internalID");
            if (string.IsNullOrWhiteSpace(val))
                stepErrors.Errors.Add(new ValidationErrorDto { Key = "internalId", Message = "'internalId' must not be empty." });
        }

        // 6) optional fields (just check types if present)
        if (HasProp(root, "purchaseOrderReference") && PropKind(root, "purchaseOrderReference") != JsonValueKind.String)
            stepErrors.Errors.Add(new ValidationErrorDto { Key = "purchaseOrderReference", Message = "'purchaseOrderReference' must be a string." });

        if (HasProp(root, "purchaseOrderDescription") && PropKind(root, "purchaseOrderDescription") != JsonValueKind.String)
            stepErrors.Errors.Add(new ValidationErrorDto { Key = "purchaseOrderDescription", Message = "'purchaseOrderDescription' must be a string." });

        if (HasProp(root, "salesOrderReference") && PropKind(root, "salesOrderReference") != JsonValueKind.String)
            stepErrors.Errors.Add(new ValidationErrorDto { Key = "salesOrderReference", Message = "'salesOrderReference' must be a string." });

        if (HasProp(root, "salesOrderDescription") && PropKind(root, "salesOrderDescription") != JsonValueKind.String)
            stepErrors.Errors.Add(new ValidationErrorDto { Key = "salesOrderDescription", Message = "'salesOrderDescription' must be a string." });

        if (HasProp(root, "proformaInvoiceNumber"))
        {
            if (PropKind(root, "proformaInvoiceNumber") != JsonValueKind.String)
                stepErrors.Errors.Add(new ValidationErrorDto { Key = "proformaInvoiceNumber", Message = "'proformaInvoiceNumber' must be a string." });
            else if (ReadString(root, "proformaInvoiceNumber").Length > 50)
                stepErrors.Errors.Add(new ValidationErrorDto { Key = "proformaInvoiceNumber", Message = "'proformaInvoiceNumber' max length is 50." });
        }

        // 7) payment & delivery — must be objects if present
        if (HasProp(root, "payment") && PropKind(root, "payment") != JsonValueKind.Object)
            stepErrors.Errors.Add(new ValidationErrorDto { Key = "payment", Message = "'payment' must be an object." });

        if (HasProp(root, "delivery") && PropKind(root, "delivery") != JsonValueKind.Object)
            stepErrors.Errors.Add(new ValidationErrorDto { Key = "delivery", Message = "'delivery' must be an object." });

        // 8) invoiceLines (required array with at least one item)
        if (!HasProp(root, "invoiceLines") || PropKind(root, "invoiceLines") != JsonValueKind.Array)
        {
            stepErrors.Errors.Add(new ValidationErrorDto { Key = "invoiceLines", Message = "Missing or invalid 'invoiceLines' array." });
        }
        else
        {
            var arr = root.GetProperty("invoiceLines");
            if (arr.GetArrayLength() == 0)
                stepErrors.Errors.Add(new ValidationErrorDto { Key = "invoiceLines", Message = "'invoiceLines' must contain at least one item." });
            else
            {
                int idx = 0;
                foreach (var line in arr.EnumerateArray())
                {
                    // required per schema: description, itemType (GS1/EGS), itemCode, unitType, quantity, unitValue, salesTotal, total
                    if (!line.TryGetProperty("description", out var p) || p.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(p.GetString()))
                        stepErrors.Errors.Add(new ValidationErrorDto { Key = $"invoiceLines[{idx}].description", Message = "description is required and must be a string." });

                    if (!line.TryGetProperty("itemType", out var it) || it.ValueKind != JsonValueKind.String)
                        stepErrors.Errors.Add(new ValidationErrorDto { Key = $"invoiceLines[{idx}].itemType", Message = "itemType is required and must be a string (GS1 or EGS)." });
                    else
                    {
                        var itv = it.GetString();
                        if (itv != "GS1" && itv != "EGS")
                            stepErrors.Errors.Add(new ValidationErrorDto { Key = $"invoiceLines[{idx}].itemType", Message = "itemType must be 'GS1' or 'EGS'." });
                    }

                    if (!line.TryGetProperty("itemCode", out var ic) || ic.ValueKind != JsonValueKind.String)
                        stepErrors.Errors.Add(new ValidationErrorDto { Key = $"invoiceLines[{idx}].itemCode", Message = "itemCode is required and must be a string." });

                    if (!line.TryGetProperty("unitType", out var ut) || ut.ValueKind != JsonValueKind.String)
                        stepErrors.Errors.Add(new ValidationErrorDto { Key = $"invoiceLines[{idx}].unitType", Message = "unitType is required and must be a string." });

                    if (!line.TryGetProperty("quantity", out var q) || (q.ValueKind != JsonValueKind.Number) || !q.TryGetDecimal(out _))
                        stepErrors.Errors.Add(new ValidationErrorDto { Key = $"invoiceLines[{idx}].quantity", Message = "quantity is required and must be a number." });

                    // unitValue must be object and contain currencySold & amountEGP (schema requires amountEGP)
                    if (!line.TryGetProperty("unitValue", out var uv) || uv.ValueKind != JsonValueKind.Object)
                        stepErrors.Errors.Add(new ValidationErrorDto { Key = $"invoiceLines[{idx}].unitValue", Message = "unitValue is required and must be an object." });
                    else
                    {
                        if (!uv.TryGetProperty("amountEGP", out var aEGP) || aEGP.ValueKind != JsonValueKind.Number || !aEGP.TryGetDecimal(out _))
                            stepErrors.Errors.Add(new ValidationErrorDto { Key = $"invoiceLines[{idx}].unitValue.amountEGP", Message = "unitValue.amountEGP is required and must be a number." });
                    }

                    if (!line.TryGetProperty("salesTotal", out var st) || st.ValueKind != JsonValueKind.Number || !st.TryGetDecimal(out _))
                        stepErrors.Errors.Add(new ValidationErrorDto { Key = $"invoiceLines[{idx}].salesTotal", Message = "salesTotal is required and must be a number." });

                    if (!line.TryGetProperty("total", out var tot) || tot.ValueKind != JsonValueKind.Number || !tot.TryGetDecimal(out _))
                        stepErrors.Errors.Add(new ValidationErrorDto { Key = $"invoiceLines[{idx}].total", Message = "total is required and must be a number." });

                    idx++;
                }
            }
        }

        // 9) totals (top-level totals) - expected numeric fields
        if (!HasProp(root, "totalSalesAmount") || PropKind(root, "totalSalesAmount") != JsonValueKind.Number)
            stepErrors.Errors.Add(new ValidationErrorDto { Key = "totalSalesAmount", Message = "Missing or invalid 'totalSalesAmount' (must be number)." });

        if (!HasProp(root, "totalDiscountAmount") || PropKind(root, "totalDiscountAmount") != JsonValueKind.Number)
            stepErrors.Errors.Add(new ValidationErrorDto { Key = "totalDiscountAmount", Message = "Missing or invalid 'totalDiscountAmount' (must be number)." });

        if (!HasProp(root, "netAmount") || PropKind(root, "netAmount") != JsonValueKind.Number)
            stepErrors.Errors.Add(new ValidationErrorDto { Key = "netAmount", Message = "Missing or invalid 'netAmount' (must be number)." });

        if (!HasProp(root, "taxTotals") || PropKind(root, "taxTotals") != JsonValueKind.Array)
            stepErrors.Errors.Add(new ValidationErrorDto { Key = "taxTotals", Message = "Missing or invalid 'taxTotals' (must be array)." });

        if (!HasProp(root, "totalAmount") || PropKind(root, "totalAmount") != JsonValueKind.Number)
            stepErrors.Errors.Add(new ValidationErrorDto { Key = "totalAmount", Message = "Missing or invalid 'totalAmount' (must be number)." });

        // optional amounts (extraDiscountAmount, totalItemsDiscountAmount) if present must be numbers
        if (HasProp(root, "extraDiscountAmount") && PropKind(root, "extraDiscountAmount") != JsonValueKind.Number)
            stepErrors.Errors.Add(new ValidationErrorDto { Key = "extraDiscountAmount", Message = "'extraDiscountAmount' must be a number if present." });

        if (HasProp(root, "totalItemsDiscountAmount") && PropKind(root, "totalItemsDiscountAmount") != JsonValueKind.Number)
            stepErrors.Errors.Add(new ValidationErrorDto { Key = "totalItemsDiscountAmount", Message = "'totalItemsDiscountAmount' must be a number if present." });

        // 10) signatures - array with at least one (issuer)
        if (!HasProp(root, "signatures") || PropKind(root, "signatures") != JsonValueKind.Array)
        {
            stepErrors.Errors.Add(new ValidationErrorDto { Key = "signatures", Message = "Missing or invalid 'signatures' array." });
        }
        else
        {
            var s = root.GetProperty("signatures");
            if (s.GetArrayLength() == 0)
                stepErrors.Errors.Add(new ValidationErrorDto { Key = "signatures", Message = "At least one signature is required." });
            else
            {
                // ensure signature objects have type & value
                int si = 0;
                foreach (var sg in s.EnumerateArray())
                {
                    if (sg.ValueKind != JsonValueKind.Object)
                        stepErrors.Errors.Add(new ValidationErrorDto { Key = $"signatures[{si}]", Message = "Signature must be an object." });
                    else
                    {
                        if (!sg.TryGetProperty("type", out var t) || t.ValueKind != JsonValueKind.String)
                            stepErrors.Errors.Add(new ValidationErrorDto { Key = $"signatures[{si}].type", Message = "Signature.type is required and must be a string (I or S)." });
                        if (!sg.TryGetProperty("value", out var v) || v.ValueKind != JsonValueKind.String)
                            stepErrors.Errors.Add(new ValidationErrorDto { Key = $"signatures[{si}].value", Message = "Signature.value is required and must be a base64 string." });
                    }
                    si++;
                }
            }
        }

        // 11) issuer details (nested)
        if (HasProp(root, "issuer") && PropKind(root, "issuer") == JsonValueKind.Object)
        {
            var issuer = root.GetProperty("issuer");
            if (!issuer.TryGetProperty("type", out var itype) || itype.ValueKind != JsonValueKind.String)
                stepErrors.Errors.Add(new ValidationErrorDto { Key = "issuer.type", Message = "issuer.type is required and must be a string (B, P, F)." });

            if (!issuer.TryGetProperty("id", out var iid) || iid.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(iid.GetString()))
                stepErrors.Errors.Add(new ValidationErrorDto { Key = "issuer.id", Message = "issuer.id (registration number) is required." });

            if (!issuer.TryGetProperty("name", out var iname) || iname.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(iname.GetString()))
                stepErrors.Errors.Add(new ValidationErrorDto { Key = "issuer.name", Message = "issuer.name is required." });

            if (!issuer.TryGetProperty("address", out var iaddr) || iaddr.ValueKind != JsonValueKind.Object)
                stepErrors.Errors.Add(new ValidationErrorDto { Key = "issuer.address", Message = "issuer.address is required and must be an object." });
            else
            {
                // branchId mandatory when issuer.type == "B"
                var it = issuer.TryGetProperty("type", out var itp) && itp.ValueKind == JsonValueKind.String ? itp.GetString() : null;
                if (it == "B")
                {
                    if (!iaddr.TryGetProperty("branchId", out var br) || br.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(br.GetString()))
                        stepErrors.Errors.Add(new ValidationErrorDto { Key = "issuer.address.branchId", Message = "issuer.address.branchId is required for issuer.type 'B'." });
                    // country must be EG for internal business issuers
                    if (!iaddr.TryGetProperty("country", out var country) || country.ValueKind != JsonValueKind.String || country.GetString() != "EG")
                        stepErrors.Errors.Add(new ValidationErrorDto { Key = "issuer.address.country", Message = "issuer.address.country must be 'EG' for internal business issuer." });
                }
            }
        }

        // 12) receiver details (nested) - minimal core rules: receiver.type required; if type == B require id & name
        if (HasProp(root, "receiver") && PropKind(root, "receiver") == JsonValueKind.Object)
        {
            var receiver = root.GetProperty("receiver");

            if (!receiver.TryGetProperty("type", out var rtype) || rtype.ValueKind != JsonValueKind.String)
                stepErrors.Errors.Add(new ValidationErrorDto { Key = "receiver.type", Message = "receiver.type is required and must be a string (B, P, F)." });
            else
            {
                var rt = rtype.GetString();
                if (rt == "B")
                {
                    if (!receiver.TryGetProperty("id", out var rid) || rid.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(rid.GetString()))
                        stepErrors.Errors.Add(new ValidationErrorDto { Key = "receiver.id", Message = "receiver.id (registration number) is required for receiver.type 'B'." });
                    if (!receiver.TryGetProperty("name", out var rname) || rname.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(rname.GetString()))
                        stepErrors.Errors.Add(new ValidationErrorDto { Key = "receiver.name", Message = "receiver.name is required for receiver.type 'B'." });
                }
                // note: for receiver.type == P (person), ID / name may be conditionally required depending on invoice amount threshold — handled by National ID Validator / business rules step.
            }

            if (receiver.TryGetProperty("address", out var raddr) && raddr.ValueKind != JsonValueKind.Object)
                stepErrors.Errors.Add(new ValidationErrorDto { Key = "receiver.address", Message = "receiver.address must be an object when present." });
        }

        // 13) serviceDeliveryDate if present must be date string
        if (HasProp(root, "serviceDeliveryDate"))
        {
            var s = ReadString(root, "serviceDeliveryDate");
            if (!DateTimeOffset.TryParse(s, out _))
                stepErrors.Errors.Add(new ValidationErrorDto { Key = "serviceDeliveryDate", Message = "serviceDeliveryDate must be a valid date string." });
        }
        return await Task.FromResult(stepErrors);
    }
}
