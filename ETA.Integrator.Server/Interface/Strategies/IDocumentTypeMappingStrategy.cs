using ETA.Integrator.Server.Dtos.SerializationDTOs;
using ETA.Integrator.Server.Models;
using ETA.Integrator.Server.Models.Consumer.ETA;

namespace ETA.Integrator.Server.Interface.Strategies
{
    public interface IDocumentMappingStrategy
    {
        InvoiceModel DocumentMap(DocumentMappIngPropertiesModel mappingProperties);
        InvoiceToSerializeDTO SerializedVersionMap(InvoiceModel model, List<string>? references  = null);
    }
}
