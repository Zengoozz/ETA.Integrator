using ETA.Integrator.Server.Models;
using ETA.Integrator.Server.Models.Consumer.ETA;

namespace ETA.Integrator.Server.Interface.Services
{
    public interface IDocumentMapper
    {
        InvoiceModel BaseMap(DocumentMappIngPropertiesModel mappingProperties);
    }
}
