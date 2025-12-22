using ETA.Integrator.Server.Dtos.SerializationDTOs;
using ETA.Integrator.Server.Helpers;
using ETA.Integrator.Server.Interface.Strategies;
using ETA.Integrator.Server.Models;
using ETA.Integrator.Server.Models.Consumer.ETA;

namespace ETA.Integrator.Server.Mapping
{
    public class InvoiceMapper : IDocumentMappingStrategy
    {
        public InvoiceModel DocumentMap(DocumentMappIngPropertiesModel mappingProperties)
        {
            InvoiceModel invoiceModel = GenericHelpers.MapBaseDocument(mappingProperties);

            return invoiceModel;
        }

        public InvoiceToSerializeDTO SerializedVersionMap(InvoiceModel model, List<string>? references)
        {
            InvoiceToSerializeDTO serialized = GenericHelpers.MapBaseSerialized(model);
            
            return serialized;
        }
    }
}
