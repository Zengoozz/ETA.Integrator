using ETA.Integrator.Server.Models.Consumer.ETA;
using ETA.Integrator.Server.Models.Provider;

namespace ETA.Integrator.Server.Models
{
    public class SigningPropertiesModel
    {
        public List<ProviderInvoiceViewModel> Documents { get; set; } = new();
        public IssuerModel Issuer { get; set; } = new();
        public string ItemCode { get; set; } = string.Empty;
        public string InvoiceType { get; set; } = string.Empty;
        public string TokenPin { get; set; } = string.Empty;
        public bool IsProduction { get; set; } = false;
        public bool ForNote { get; set; } = false;
    }
}
