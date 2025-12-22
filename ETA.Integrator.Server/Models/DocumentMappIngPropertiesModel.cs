using ETA.Integrator.Server.Models.Consumer.ETA;
using ETA.Integrator.Server.Models.Provider;

namespace ETA.Integrator.Server.Models
{
    public class DocumentMappIngPropertiesModel
    {
        public ProviderInvoiceViewModel Document { get; set; } = new();
        public IssuerModel Issuer { get; set; } = new();
        public string ItemCode { get; set; } = string.Empty;
        public string InvoiceType { get; set; } = string.Empty;
        public bool IsProduction { get; set; } = false;
        public bool ForNotes { get; set; } = false;
        public List<string> References { get; set; } = new();
        public string DocumentType
        {
            get
            {
                return ForNotes ? "CreditNote" : "Invoice";
            }
        }
    }
}
