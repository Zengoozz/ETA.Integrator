using ETA.Integrator.Server.Models.Consumer.ETA;
using ETA.Integrator.Server.Models.Provider;

namespace ETA.Integrator.Server.Interface.Services
{
    public interface IInvoiceService
    {
        InvoiceModel PrepareInvoiceData(ProviderInvoiceViewModel invoiceViewModel, IssuerModel issuer);
    }
}
