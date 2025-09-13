using ETA.Integrator.Server.Models.Consumer.ETA;

namespace ETA.Integrator.Server.Interface.Services.Consumer
{
    public interface ISignatureConsumerService
    {
        string SignDocument(InvoiceModel model, string tokenPin);
    }
}
