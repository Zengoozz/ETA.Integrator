

using System.Text.Json.Serialization;

namespace ETA.EInvoicing.Validator.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum InvoiceValidationType : byte
{
    Structure,
    CoreFields,
    Signature,
    NationalID,
    Taxpayer,
    ReferenceDocument,
    Code,
    SimpleFields
}