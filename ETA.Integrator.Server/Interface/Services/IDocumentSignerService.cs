using ETA.Integrator.Server.Models;

namespace ETA.Integrator.Server.Interface.Services
{
    public interface IDocumentSignerService
    {
        List<string> SignMultipleDocuments(SigningPropertiesModel signingProperties);
        List<string> SignMultipleDocumentsMock(SigningPropertiesModel signingProperties);
    }
}
