using ETA.Integrator.Server.Models.Provider;

namespace ETA.Integrator.Server.Models
{
    public class InvoiceRequest
    {

        public List<ProviderInvoiceViewModel> Invoices { get; set; } = new();
        public string InvoiceType { get; set; } = string.Empty;
    }
}
