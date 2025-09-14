using ETA.Integrator.Server.Models.Consumer.ETA;
using ETA.Integrator.Server.Models.Provider;

namespace ETA.Integrator.Server.Interface.Services
{
    public interface IDocumentSignerService
    {
        string SignDocument(InvoiceModel model, string tokenPin);
        List<string> SignMultipleDocuments(List<ProviderInvoiceViewModel> viewModels, IssuerModel issuer, string invoiceType, string tokenPin);
    }
}
