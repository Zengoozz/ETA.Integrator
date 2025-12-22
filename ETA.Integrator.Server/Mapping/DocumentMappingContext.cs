using ETA.Integrator.Server.Dtos.SerializationDTOs;
using ETA.Integrator.Server.Interface.Strategies;
using ETA.Integrator.Server.Models;
using ETA.Integrator.Server.Models.Consumer.ETA;
using ETA.Integrator.Server.Models.Core;

namespace ETA.Integrator.Server.Mapping
{
    public class DocumentMappingProcessor
    {
        private readonly IServiceProvider _serviceProvider;

        public DocumentMappingProcessor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private IDocumentMappingStrategy GetMapper(string documentType)
        {
            var mapper = _serviceProvider.GetKeyedService<IDocumentMappingStrategy>(documentType);

            if (mapper == null)
            {
                throw new ProblemDetailsException(
                        statusCode: StatusCodes.Status400BadRequest,
                        message: "INVALID",
                        detail: "Document type is invalid"
                        );
            }

            return mapper;
        }

        public InvoiceModel ProcessDocumentMap(DocumentMappIngPropertiesModel props)
        {
            var mapper = GetMapper(props.DocumentType);
            return mapper.DocumentMap(props);
        }

        public InvoiceToSerializeDTO ProcessSerializedVersionMap(string documentType, InvoiceModel model, List<string>? references)
        {
            var mapper = GetMapper(documentType);
            return mapper.SerializedVersionMap(model, references);
        }

    }
}
