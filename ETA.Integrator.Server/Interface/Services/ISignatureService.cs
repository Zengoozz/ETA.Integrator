using ETA.Integrator.Server.Models.Consumer.ETA;

namespace ETA.Integrator.Server.Interface.Services
{
    public interface ISignatureService
    {
        void SignDocument(InvoiceModel model, string tokenPin);
    }
}
