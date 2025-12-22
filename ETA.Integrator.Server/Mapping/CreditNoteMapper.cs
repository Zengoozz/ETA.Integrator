using ETA.Integrator.Server.Dtos.SerializationDTOs;
using ETA.Integrator.Server.Helpers;
using ETA.Integrator.Server.Interface.Strategies;
using ETA.Integrator.Server.Models;
using ETA.Integrator.Server.Models.Consumer.ETA;
using ETA.Integrator.Server.Models.Core;

namespace ETA.Integrator.Server.Mapping
{
    public class CreditNoteMapper : IDocumentMappingStrategy
    {
        public InvoiceModel DocumentMap(DocumentMappIngPropertiesModel mappingProperties)
        {
            if (String.IsNullOrEmpty(mappingProperties.Document.ReferenceId))
                throw new ProblemDetailsException(
                    statusCode: StatusCodes.Status400BadRequest,
                    message: "INVALID",
                    detail: "Credit notes should have references"
                    );


            InvoiceModel baseModel = GenericHelpers.MapBaseDocument(mappingProperties);

            baseModel.Receiver.Type = baseModel.Receiver.Type != "B" ? "P" : "B";

            return new CreditNoteModel(baseModel)
            {
                DocumentType = "c",
                References = mappingProperties.References
            };
        }

        public InvoiceToSerializeDTO SerializedVersionMap(InvoiceModel model, List<string>? references)
        {
            if (references is null || references.Count == 0)
                throw new ProblemDetailsException(
                    statusCode: StatusCodes.Status400BadRequest,
                    message: "INVALID",
                    detail: "Credit notes should have references"
                    );

            InvoiceToSerializeDTO serialized = GenericHelpers.MapBaseSerialized(model);
            
            return new CreditNoteToSerializeDTO(serialized)
            {
                DocumentType = "c",
                References = references
            };
        }
    }
}
