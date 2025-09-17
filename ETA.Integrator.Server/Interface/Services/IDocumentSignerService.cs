using ETA.Integrator.Server.Models.Consumer.ETA;
using ETA.Integrator.Server.Models.Provider;

namespace ETA.Integrator.Server.Interface.Services
{
    public interface IDocumentSignerService
    {
        List<string> SignMultipleDocuments(List<ProviderInvoiceViewModel> viewModels, IssuerModel issuer, string invoiceType, string tokenPin);
    }
}
