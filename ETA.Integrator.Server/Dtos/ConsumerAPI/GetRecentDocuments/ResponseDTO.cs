namespace ETA.Integrator.Server.Dtos.ConsumerAPI.GetRecentDocuments
{
    public class ResponseDTO
    {
        public List<DocumentSummaryDTO> Result { get; set; } = new();
        public MetadataDTO Metadata { get; set; } = new();
    }
}
