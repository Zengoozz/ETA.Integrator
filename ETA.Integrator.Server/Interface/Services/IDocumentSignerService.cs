using ETA.Integrator.Server.Models;

namespace ETA.Integrator.Server.Interface.Services
{
    public interface IDocumentSignerService
    {
        Task<List<string>> SignMultipleDocuments(SigningPropertiesModel signingProperties);
        Task<List<string>> SignMultipleDocumentsMock(SigningPropertiesModel signingProperties);
    }
}
