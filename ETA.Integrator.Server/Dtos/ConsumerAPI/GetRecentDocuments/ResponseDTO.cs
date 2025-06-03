namespace ETA.Integrator.Server.Dtos.ConsumerAPI.GetRecentDocuments
{
    public class ResponseDTO
    {
        public MetadataDTO Metadata { get; set; } = new();
        public DocumentSummaryDTO Result { get; set; } = new();
    }
}
